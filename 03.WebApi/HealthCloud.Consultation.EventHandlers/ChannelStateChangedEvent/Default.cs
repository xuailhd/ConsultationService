using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Common.Utility;
using HealthCloud.Consultation.Dto.IM;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelStateChangedEvent
{
    public class DefaultHandler : IEventHandler<Dto.EventBus.ChannelStateChangedEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();
        ConversationIMUidService uidService = new ConversationIMUidService();
        IIMHepler imService = new Services.QQCloudy.IMHelper();

        public bool Handle(Dto.EventBus.ChannelStateChangedEvent evt)
        {
            ResponseConversationRoomDTO room = null;

            try
            {
                if (evt == null)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(evt.FromUserID))
                {
                    return true;
                }

                //获取房间信息
                room = roomService.GetChannelInfo(evt.ChannelID);

                #region  参数校验：房间不存在的则不允许修改
                if (room == null)
                {
                    return true;
                }
                #endregion


                var CurrentOperatorUserIdentifier = evt.FromUseridentifier;

                var RoomStateChangeMsgDesc = "";
                var RoomOperatorType = "";
                var RoomOperatorRemark = "";

                //结束看诊
                if (evt.State == EnumRoomState.AlreadyVisit)
                {
                    if (evt.UserType == EnumUserType.Doctor)
                    {
                        #region 停止计费
                        if (room.ChargingState == EnumRoomChargingState.Started && !roomService.PauseCharging(evt.ChannelID))
                        {
                            return false;
                        }
                        #endregion

                        #region 更新订单状态

                        if (room.ServiceType == EnumDoctorServiceType.AudServiceType ||
                        room.ServiceType == EnumDoctorServiceType.VidServiceType ||
                        room.ServiceType == EnumDoctorServiceType.PicServiceType ||
                        room.ServiceType == EnumDoctorServiceType.Consultation)
                        {
                            //订单完成
                            throw new NotImplementedException("未实现订单完成");
                            //if (!orderService.Complete("", room.ServiceID))
                            //{
                            //    KMEHosp.Common.LogHelper.WriteWarn($"订单完成失败,ServiceID={room.ServiceID}");
                            //    return false;
                            //}
                        }
                        #endregion

                        #region 提交处方签名
                        using (MQChannel mqChannel = new MQChannel())
                        {
                            if (!mqChannel.Publish(new Dto.EventBus.RecipeSignSubmitEvent()
                            {
                                ServiceID = room.ServiceID
                            }))
                            {
                                LogHelper.DefaultLogger.Error($"发布RecipeSignSubmitEvent失败,ServiceID={room.ServiceID}");
                                return false;
                            }
                        }
                        #endregion

                        #region 更新监控指标（记录服务时长，总耗时，就诊是否结束标志）                      

                        SysMonitorIndexService service = new SysMonitorIndexService();
                        var values = new Dictionary<string, string>();
                        values.Add("VisitingServiceChargingState", room.ChargingState.ToString());//就诊暂停标志
                        values.Add("VisitingServiceDurationSeconds", room.Duration.ToString());//就诊服务时长
                        values.Add("VisitingServiceElapsedSeconds", room.TotalTime.ToString());//就诊消耗时长                            
                        values.Add("VisitingRoomState", room.RoomState.ToString());//就诊暂停标志
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

                        //语音、视频看诊
                        if (room.ServiceType == EnumDoctorServiceType.AudServiceType ||
                            room.ServiceType == EnumDoctorServiceType.VidServiceType)
                        {
                            var DoctorID = "";

                            #region 获取医生编号

                            if (room.ServiceType == EnumDoctorServiceType.AudServiceType || room.ServiceType == EnumDoctorServiceType.VidServiceType)
                            {
                                UserOPDRegisterService bllOPD = new UserOPDRegisterService();

                                //获取预约信息
                                //var opd = bllOPD.Single<UserOPDRegister>(room.ServiceID);
                                var triage = bllOPD.GetDoctorTriageDetail(room.ServiceID);

                                if (triage != null)
                                {
                                    DoctorID = triage.TriageDoctorID;
                                }
                                else
                                {
                                    LogHelper.DefaultLogger.Error("房间 " + room.ConversationRoomID + " 对应的预约记录不存在");
                                }
                            }

                            #endregion

                            var DoctorUid = uidService.GetUserIMUid(DoctorID);

                            #region 发送候诊队列通知
                            roomService.SendWaitingQueueChangeNotice(DoctorID);
                            #endregion
                        }

                        RoomOperatorRemark = $"";
                        RoomOperatorType = "Hangup";
                        RoomStateChangeMsgDesc = "医生已结束看诊，请对本次服务作出评价";
                    }
                    else
                    {
                        //无效请求
                        return true;
                    }
                }
                else if (evt.State == EnumRoomState.Waiting)
                {
                    //取消呼叫
                    if (evt.UserType == EnumUserType.Doctor)
                    {
                        RoomStateChangeMsgDesc = "医生取消了呼叫";
                        RoomOperatorType = "Call_Cancel";

                    }
                    else
                    {
                        RoomStateChangeMsgDesc = "患者正在候诊，等待医生呼叫";
                        RoomOperatorType = "Wait";

                        #region 修改状态并设置分诊编号
                        room.RoomState = evt.State;
                        room.TriageID = SeqIDHelper.GetSeqId();

                        //修改就诊时间和开始就诊时间
                        if (!roomService.UpdateChannelState(room))
                        {
                            LogHelper.DefaultLogger.Error($"修改房间信息失败,ConversationRoomID={room.ConversationRoomID}");
                            return false;
                        }
                        #endregion

                        #region 发送患者进入诊室的通知
                        if (evt.UserType == EnumUserType.User || evt.UserType == EnumUserType.Drugstore)
                        {
                            using (MQChannel mqChannel = new MQChannel())
                            {
                                if (!mqChannel.Publish(new Dto.EventBus.UserNoticeEvent()
                                {
                                    NoticeType = EnumNoticeSecondType.DoctorVidUserEnterRoomNotice,
                                    ServiceID = room.ServiceID
                                }))
                                {
                                    LogHelper.DefaultLogger.Error($"Publis UserNoticeEvent失败,ServiceID={room.ServiceID}");
                                    return false;
                                }
                            }
                        }
                        #endregion
                    }
                }
                else if (evt.State == EnumRoomState.Calling)
                {
                    //医生呼叫患者
                    if (evt.UserType == EnumUserType.Doctor)
                    {
                        RoomOperatorType = "Calling";
                        RoomStateChangeMsgDesc = "医生正在呼叫，等待患者接听";

                        using (MQChannel mqChannel = new MQChannel())
                        {
                            if (!mqChannel.Publish(new Dto.EventBus.UserNoticeEvent()
                            {
                                NoticeType = EnumNoticeSecondType.UserVidDoctorCallNotice,
                                ServiceID = room.ServiceID
                            }))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //无效请求
                        return true;
                    }
                }
                else if (evt.State == EnumRoomState.InMedicalTreatment)
                {
                    if (evt.UserType == EnumUserType.Doctor)
                    {
                        //无效请求
                        return true;
                    }
                    //患者接听
                    else
                    {
                        RoomOperatorType = "Calling_Answer";
                        RoomStateChangeMsgDesc = "患者已进入诊室";

                        #region 修改订单状态，用户将不能够再取消订单                 

                        if (room.ServiceType == EnumDoctorServiceType.AudServiceType ||
                        room.ServiceType == EnumDoctorServiceType.VidServiceType)
                        {
                            #region 语音/视频 修改订单状态（配送中）                           
                            //if (!orderService.LogisticWithDistributionIn(room.ServiceID))
                            //    return false;
                            #endregion
                        }
                        else if (room.ServiceType == EnumDoctorServiceType.PicServiceType)
                        {
                            #region 图文咨询 修改订单状态（配送中）

                            //BLL.OrderService bllOrder = new OrderService(evt.FromUserID);
                            //if (!bllOrder.LogisticWithDistributionIn(room.ServiceID))
                            //    return false;
                            #endregion

                        }
                        else if (room.ServiceType == EnumDoctorServiceType.Consultation)
                        {
                            #region 更改会诊状态
                            //var service = new BLL.RemoteConsultation.Implements.RemoteConsultationService();
                            //var res = service.Start(room.ServiceID, evt.FromUserID, "");
                            //if (res == EnumApiStatus.ConsultationNotExists || res == EnumApiStatus.ConsultationNotChangeProgress)
                            //    return true;
                            //if (res == EnumApiStatus.BizError)
                            //    return false;
                            #endregion

                            #region 修改状态
                            room.RoomState = evt.State;
                            //修改就诊时间和开始就诊时间
                            if (!roomService.UpdateChannelState(room))
                            {
                                return false;
                            }
                            #endregion
                        }



                        #endregion

                        #region 计算候诊耗时并更新统计指标
                        var log = roomService.GetChannelLastLog(room.ConversationRoomID, "Wait");
                        if (log != null)
                        {
                            //候诊耗时
                            var WaitingElapsedSeconds = (DateTime.Now - log.OperationTime).TotalSeconds;

                            RoomOperatorRemark = $"用户候诊用时{WaitingElapsedSeconds}秒";

                            SysMonitorIndexService service = new SysMonitorIndexService();
                            var values = new Dictionary<string, string>();
                            values.Add("WaitingElapsedSeconds", WaitingElapsedSeconds.ToString());

                            //更新候诊总耗时，指标=原指标+新指标
                            if (!service.InsertAndUpdate(new RequestSysMonitorIndexUpdateDTO()
                            {
                                Category = "UserConsult",
                                OutID = room.ServiceID,
                                Values = values
                            }, true))
                            {
                                return false;
                            }
                        }


                        #endregion
                    }
                }
                else if (evt.State == EnumRoomState.NoTreatment)
                {
                    if (evt.UserType == EnumUserType.Doctor)
                    {
                        RoomOperatorType = "Calling_Cancel";
                        RoomStateChangeMsgDesc = "医生取消了呼叫";
                    }
                    else
                    {
                        RoomOperatorType = "Waiting_Cancel";
                        RoomStateChangeMsgDesc = "患者取消了候诊";
                    }

                    //发送指令前状态是在就诊中
                    if (evt.ExpectedState == EnumRoomState.InMedicalTreatment)
                    {
                        #region 停止计费
                        if (room.ChargingState == EnumRoomChargingState.Started && !roomService.PauseCharging(evt.ChannelID))
                        {
                            return false;
                        }
                        #endregion
                    }

                }
                else if (evt.State == EnumRoomState.Disconnection)
                {
                    if (evt.UserType == EnumUserType.Doctor)
                    {
                        RoomOperatorType = "Leave";
                        RoomStateChangeMsgDesc = "医生已离开";
                    }
                    else
                    {
                        RoomOperatorType = "Leave";
                        RoomStateChangeMsgDesc = "患者已离开";
                    }

                    #region 停止计费

                    if (room.ChargingState == EnumRoomChargingState.Started && !roomService.PauseCharging(evt.ChannelID))
                    {
                        return false;
                    }
                    #endregion

                }
                else if (evt.State == EnumRoomState.WaitAgain)
                {
                    RoomOperatorType = "Waiting";
                    RoomStateChangeMsgDesc = "患者正在候诊，等待医生呼叫";
                }

                if (room.Enable)
                {
                    var State = room.RoomState;

                    //兼容移动端状态
                    if (State == EnumRoomState.WaitAgain)
                    {
                        State = EnumRoomState.Waiting;
                    }

                    ///写入日志
                    if (!roomService.InsertChannelLog(room.ConversationRoomID,
                        room.UserID,
                        room.MemberName,
                        RoomOperatorType,
                        RoomStateChangeMsgDesc,
                        RoomOperatorRemark))
                    {
                        return false;
                    }

                    if (!imService.SendGroupCustomMsg(evt.ChannelID, CurrentOperatorUserIdentifier, new RequestCustomMsgRoomStateChanged()
                    {
                        Data = new RequestConversationRoomStatusDTO()
                        {
                            ChannelID = evt.ChannelID,
                            State = State,
                            ServiceID = room.ServiceID,
                            ServiceType = room.ServiceType,
                            Duration = room.Duration,
                            ChargingState = room.ChargingState,
                            TotalTime = room.TotalTime,
                            DisableWebSdkInteroperability = room.DisableWebSdkInteroperability
                        },
                        Desc = RoomStateChangeMsgDesc
                    }))
                    {
                        return false;
                    }

                }
            }
            catch (Exception E)
            {
                LogHelper.DefaultLogger.Error(E.Message, E);
                return false;
            }

            return true;
        }
    }


}
