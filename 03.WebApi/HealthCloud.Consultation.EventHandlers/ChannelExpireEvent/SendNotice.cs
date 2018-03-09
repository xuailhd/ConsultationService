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

namespace HealthCloud.Consultation.EventHandlers.ChannelExpireEvent
{
    /// <summary>
    /// 发送时长变更通知给客户端
    /// 
    /// 前置条件：订单支付完成
    /// 后置条件：修改房间的服务时长到存储
    /// </summary>
    public class SendNotice : IEventHandler<Dto.EventBus.ChannelExpireEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();
        IIMHepler imService = new IMHelper();

        public bool Handle(Dto.EventBus.ChannelExpireEvent evt)
        {
            try
            {
                var room = roomService.GetChannelInfo(evt.ServiceID);

                //频道存在且没有结束才提示，如果已经结束就诊则不需要再提示用户续费
                if (room != null && room.RoomState!= EnumRoomState.AlreadyVisit)
                {
                    //只发布续费提醒消息
                    var doctorUid = roomService.GetChannelUsersInfo(room.ConversationRoomID).FirstOrDefault(a => a.UserType == EnumUserType.Doctor);                        

                    return imService.SendGroupCustomMsg(room.ConversationRoomID, doctorUid.identifier, new RequestCustomMsgRoomExpire()
                    {
                        Data = new RequestConversationRoomRenewUpgradeDTO()
                        {
                            ChannelID = room.ConversationRoomID,
                            State = room.RoomState,
                            ServiceID = room.ServiceID,
                            ServiceType = room.ServiceType,
                            //RenewOrderNo = renewOrder.OrderNo
                        },
                        Desc = $"本次服务即将结束，请即时续费"
                    });

                }
                else
                {
                    return true;
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

