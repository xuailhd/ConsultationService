using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.IM;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Services.QQCloudy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelChargingEvent
{
    /// <summary>
    /// 进入房间接通后开始计费
    /// 
    /// 触发条件：图文咨询医生首次回复，语音视频咨询医生呼叫患者患者接听时
    /// 前置条件：房间已经启用
    /// 后置条件：修改频道消耗的时长和频道状态
    /// </summary>
    public class DefaultHandler : IEventHandler<Dto.EventBus.ChannelChargingEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();
        UserOPDRegisterService opdService = new UserOPDRegisterService();
        IIMHepler imService = new IMHelper();


        public bool Handle(Dto.EventBus.ChannelChargingEvent evt)
        {
            try
            {
                if (evt == null)
                {
                    return true;
                }

                var room = roomService.GetChannelInfo(evt.ChannelID);

                #region 校验：就诊已经结束则停止计费
                if (room == null || room.RoomState == EnumRoomState.AlreadyVisit)
                {
                    return true;
                }
                #endregion

                #region 校验：计费已经停止则停止计费
                if (room.ChargingState == EnumRoomChargingState.Stoped)
                {
                    return true;
                }
                #endregion

                #region 校验：计费已经暂停则停止计费
                if (room.ChargingState == EnumRoomChargingState.Paused)
                {
                    return true;
                }
                #endregion

                #region 校验：计费没有开始则停止计费
                if (room.ChargingState != EnumRoomChargingState.Started)
                {
                    return true;
                }
                #endregion

                using (MQChannel mqChannel = new MQChannel())
                {
                    room.TotalTime += evt.Interval;//总消耗

                    #region 更新监控指标（记录服务时长，总耗时，就诊是否结束标志）     

                    SysMonitorIndexService service = new SysMonitorIndexService();
                    var values = new Dictionary<string, string>();
                    values.Add("VisitingServiceChargingState", room.ChargingState.ToString());//就诊暂停标志
                    values.Add("VisitingServiceDurationSeconds", room.Duration.ToString());//就诊服务时长
                    values.Add("VisitingServiceElapsedSeconds", room.TotalTime.ToString());//就诊消耗时长                            
                    values.Add("VisitingRoomState", room.RoomState.ToString());//就诊消耗时长 

                    if (!service.InsertAndUpdate(new RequestSysMonitorIndexUpdateDTO()
                    {
                        Category = "UserConsult",
                        OutID = room.ServiceID,
                        Values = values
                    }))
                    {
                        return false;
                    }
                    
                    #endregion

                    //使用逻辑时间，分布式系统下存在时钟不同步问题，通过上次计费的时间增加15秒得到当前时间
                    var Now = evt.ChargingTime.AddSeconds(evt.Interval);
                    room.ChargingSeq = evt.Seq;//时钟序号
                    room.ChargingTime = Now;//计费时间
                    room.ChargingInterval = evt.Interval;//时钟周期

                    //计费结束
                    if ((room.TotalTime >= room.Duration && room.Duration > 0) || (room.Duration <= 0 && room.TotalTime > 60 * 30))
                    {
                        #region 计费结束
                        room.RoomState = EnumRoomState.AlreadyVisit;
                        room.EndTime = Now;//这里使用逻辑时间（不要使用系统时间）

                        if (roomService.UpdateChannelChargeSeq(room.ConversationRoomID, room.ChargingSeq, room.ChargingTime,
                            room.ChargingInterval,EnumRoomState.AlreadyVisit,Now))
                        {
                            var DoctorUid = roomService.GetChannelUsersInfo(room.ConversationRoomID).FirstOrDefault(a => a.UserType == EnumUserType.Doctor);

                            if (DoctorUid == null)
                            {
                                return false;
                            }
                            else
                            {
                                #region 更新订单状态

                                //订单完成
                                if (!opdService.OPDComplete(room.ServiceID))
                                {
                                    return false;
                                }
                                #endregion

                                //语音、视频看诊
                                if (room.ServiceType == EnumDoctorServiceType.AudServiceType ||
                                    room.ServiceType == EnumDoctorServiceType.VidServiceType)
                                {
                                    #region 发送频道房间挂断消息
                                       
                                    if (!imService.SendGroupCustomMsg(evt.ChannelID, DoctorUid.identifier, new RequestIMCustomMsgRoomHangup()
                                    {
                                        Data = new RequestConversationRoomStatusDTO()
                                        {

                                            ChannelID = room.ConversationRoomID,
                                            Duration = room.Duration,
                                            ServiceID = room.ServiceID,
                                            ServiceType = room.ServiceType,
                                            State = room.RoomState,
                                            TotalTime = room.TotalTime,
                                            DisableWebSdkInteroperability = room.DisableWebSdkInteroperability,
                                        },
                                        Desc = "本次就诊已结束"
                                    }))
                                    {
                                        return false;
                                    }
                                    #endregion
                                }
                                //图文咨询
                                else if (room.ServiceType == EnumDoctorServiceType.PicServiceType)
                                {
                                    #region 发送频道状态变更消息
                                    if (!imService.SendGroupCustomMsg(evt.ChannelID, DoctorUid.identifier, new RequestCustomMsgRoomStateChanged()
                                    {
                                        Data = new RequestConversationRoomStatusDTO()
                                        {
                                            ChannelID = evt.ChannelID,
                                            DisableWebSdkInteroperability = room.DisableWebSdkInteroperability,
                                            State = EnumRoomState.AlreadyVisit,
                                            ServiceID = room.ServiceID,
                                            ServiceType = room.ServiceType,
                                            Duration = room.Duration
                                        },
                                        Desc = "本次咨询已结束"
                                    }))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                    #endregion

                                }
                                //远程会诊
                                else if (room.ServiceType == EnumDoctorServiceType.Consultation)
                                {
                                    #region 发送频道状态变更消息
                                    if (!imService.SendGroupCustomMsg(evt.ChannelID, DoctorUid.identifier, new RequestCustomMsgRoomStateChanged()
                                    {
                                        Data = new RequestConversationRoomStatusDTO()
                                        {
                                            ChannelID = evt.ChannelID,
                                            DisableWebSdkInteroperability = room.DisableWebSdkInteroperability,
                                            State = EnumRoomState.AlreadyVisit,
                                            ServiceID = room.ServiceID,
                                            ServiceType = room.ServiceType,
                                            Duration = room.Duration
                                        },
                                        Desc = "本次会诊已结束"
                                    }))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                    #endregion
                                }

                                return true;
                            }
                        }
                        #endregion
                    }
                    //不限制时长，会长期占用服务端资源。现在计费之前都会设置默认时长。在没有设置默认时长的时候则是小于0的数据
                    else if (room.Duration <= 0)
                    {
                        #region 计费中
                        if (roomService.UpdateChannelChargeSeq(room.ConversationRoomID, room.ChargingSeq, room.ChargingTime,
                            room.ChargingInterval))
                        {
                            //发布延时消息，15秒为一个周期。消费端收到消息后重新计算房间已通话时间。
                            return mqChannel.Publish(new Dto.EventBus.ChannelChargingEvent()
                            {
                                ChannelID = evt.ChannelID,
                                Seq = evt.Seq + 1,
                                ChargingTime = Now,
                                Interval = evt.Interval
                            }, evt.Interval);
                        }
                        else
                        {
                            return false;
                        }
                        #endregion
                    }
                    //计费未结束，继续计费
                    else if (room.TotalTime < room.Duration)
                    {
                        #region 计费中
                        if (roomService.UpdateChannelChargeSeq(room.ConversationRoomID, room.ChargingSeq, room.ChargingTime,
                            room.ChargingInterval))
                        {
                            var Interval = room.Duration - room.TotalTime;

                            mqChannel.BeginTransaction();

                            #region 检查：是否需要发送续费消息.接近一分钟时发送
                            var Duration = (room.Duration <= 0 ? 0 : room.Duration);
                            var TotalTime = room.TotalTime > Duration ? Duration : room.TotalTime;

                            if (room.ChargingSeq == (room.Duration / room.ChargingInterval) - (60 / room.ChargingInterval))
                            {
                                //获取患者信息
                                var otherUser = roomService.GetChannelUsersInfo(room.ConversationRoomID).Where(a => a.UserType != EnumUserType.Doctor);

                                #region 药店不需要发送续费消息
                                if (!otherUser.Any(a => a.UserType == EnumUserType.Drugstore))
                                {
                                    if (!mqChannel.Publish(new Dto.EventBus.ChannelExpireEvent()
                                    {
                                        ServiceID = room.ServiceID

                                    }))
                                    {
                                        return false;
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            //剩余时间大于一个时钟周期，那么按照正常时钟15秒处理
                            if (Interval > evt.Interval)
                            {
                                //发布延时消息，15秒为一个周期。消费端收到消息后重新计算房间已通话时间。
                                if (!mqChannel.Publish(new Dto.EventBus.ChannelChargingEvent()
                                {
                                    ChannelID = evt.ChannelID,
                                    Seq = evt.Seq + 1,
                                    ChargingTime = Now,
                                    Interval = evt.Interval
                                }, evt.Interval))
                                {
                                    return false;
                                }

                            }
                            //如果小于小于时钟周期那么，按照剩余时间执行
                            else
                            {
                                //发布延时消息，15秒为一个周期。消费端收到消息后重新计算房间已通话时间。
                                if (!mqChannel.Publish(new Dto.EventBus.ChannelChargingEvent()
                                {
                                    ChannelID = evt.ChannelID,
                                    Seq = evt.Seq + 1,
                                    ChargingTime = Now,
                                    Interval = evt.Interval
                                }, evt.Interval))
                                {
                                    return false;
                                }
                            }

                            mqChannel.Commit();

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex);
               
            }

            return false;
        }
    }
}
