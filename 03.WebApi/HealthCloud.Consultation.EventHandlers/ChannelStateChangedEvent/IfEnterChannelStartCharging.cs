using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelStateChangedEvent
{
    /// <summary>
    /// 进入房间接通后开始计费
    /// 作者：郭明
    /// 日期：2017年4月27日
    /// </summary>
    public class IfEnterChannelStartCharging : IEventHandler<Dto.EventBus.ChannelStateChangedEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();

        public bool Handle(Dto.EventBus.ChannelStateChangedEvent evt)
        {

            if (evt.State == EnumRoomState.InMedicalTreatment)
            {
                var LockName = $"{typeof(IfEnterChannelStartCharging)}:{evt.ChannelID}";
                var lockValue = Guid.NewGuid().ToString("N");

                //获取分布式锁，获取锁失败时进行锁等待（锁超时时间5秒）
                if (LockName.Lock(lockValue, TimeSpan.FromSeconds(5)))
                {
                    try
                    {
                        var room = roomService.GetChannelInfo(evt.ChannelID);

                        if (room != null)
                        {
                            if (room.ChargingState == EnumRoomChargingState.Stoped)
                            {
                                return roomService.StartCharging(room.ConversationRoomID, room.Duration, room.ServiceID, room.OrderNo);
                            }
                            else if (room.ChargingState == EnumRoomChargingState.Paused)
                            {
                                return roomService.RestartCharging(evt.ChannelID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                        return false;
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }
                }
                else
                {
                    //获取锁失败
                    return false;
                }
            }

            return true;

        }
    }
}
