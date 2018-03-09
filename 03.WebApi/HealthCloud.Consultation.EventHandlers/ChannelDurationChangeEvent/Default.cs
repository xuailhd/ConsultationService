using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.IM;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelDurationChangeEvent
{
    /// <summary>
    /// 发送时长变更通知给客户端
    /// 
    /// 触发条件：
    /// 1、视频或语音看诊，第一次进入诊室时
    /// 2、续费订单支付后
    /// </summary>
    public class Default : IEventHandler<Dto.EventBus.ChannelDurationChangeEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();

        string Format(int duration)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";

            if (ts.Days > 0)
            {
                str += $"{ts.Days}天";
            }

            if (ts.Hours > 0)
            {
                str += $"{ts.Hours.ToString()}小时";
            }

            if (ts.Minutes > 0)
            {
                str += $"{ts.Minutes.ToString()}分钟";
            }

            if (ts.Seconds > 0)
            {
                str += $"{ts.Seconds}秒";
            }

            return str;
        }

        public bool Handle(Dto.EventBus.ChannelDurationChangeEvent evt)
        {

            if (evt == null || string.IsNullOrEmpty(evt.OrderNo))
                return true;

            var LockName = $"{typeof(Default)}:{evt.OrderNo}:{evt.NewUpgradeOrderNo}";
            var lockValue = Guid.NewGuid().ToString("N");

            //获取分布式锁，获取锁失败时进行锁等待（锁超时时间5秒）
            if (LockName.Lock(lockValue,TimeSpan.FromSeconds(5)))
            {
                try
                {

                    var room = roomService.GetChannelInfo(evt.ServiceID);

                    #region 频道不存在，忽略无效消息
                    if (room == null || room.RoomState == EnumRoomState.AlreadyVisit)
                    {
                        //
                        return true;
                    }
                    #endregion

                    #region  已经开始计费时才发送计费消息
                    if (room.ChargingState != EnumRoomChargingState.Started)
                    {
                        return true;
                    }
                    #endregion

                    #region 修改服务时长
                    if (!roomService.IncrementChannelDuration(room.ConversationRoomID, evt.ServiceID, evt.Duration, evt.OrderNo, evt.NewUpgradeOrderNo))
                    {
                        return false;
                    }
                    #endregion

                    //重新获取房间信息
                    room = roomService.GetChannelInfo(evt.ServiceID);
                    //获取医生信息
                    var doctorUser = roomService.GetChannelUsersInfo(room.ConversationRoomID).Find(a => a.UserType == EnumUserType.Doctor);
                    var Duration = (room.Duration <= 0 ? 0 : room.Duration);
                    var TotalTime = room.TotalTime > Duration ? Duration : room.TotalTime;

                    using (MQChannel channel = new MQChannel())
                    {
                        ///处方购买消息去重复
                        var CacheKey_Derep = new StringCacheKey(StringCacheKeyType.SysDerep_OrderNewupgrade, $"{evt.OrderNo}:{evt.NewUpgradeOrderNo}");

                        //订单续费去重复
                        if (!CacheKey_Derep.FromCache<bool>())
                        {
                            #region 发送服务时长变更消息
                            if (!channel.Publish(new Dto.EventBus.ChannelSendGroupMsgEvent<RequestCustomMsgRoomDurationChanged>()
                            {
                                Msg = new RequestCustomMsgRoomDurationChanged()
                                {
                                    Data = new RequestConversationRoomStatusDTO()
                                    {
                                        ChannelID = room.ConversationRoomID,
                                        State = room.RoomState,
                                        ServiceID = room.ServiceID,
                                        ServiceType = room.ServiceType,
                                        ChargingState = room.ChargingState,
                                        Duration = Duration, //总时长
                                        TotalTime = TotalTime,// 消耗
                                        DisableWebSdkInteroperability = room.DisableWebSdkInteroperability
                                    },
                                    Desc = $"服务计时中，总时长{Format(Duration)}, 剩余{Format(Duration - TotalTime)}"
                                },
                                ChannelID = room.ConversationRoomID,
                                FromAccount = doctorUser.identifier
                            }))
                            {
                                return false;
                            }
                            #endregion

                            true.ToCache(CacheKey_Derep, TimeSpan.FromMinutes(5));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.DefaultLogger.Error(ex);
                    return false;
                }
                finally
                {
                    LockName.UnLock(lockValue);
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
