using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Services.QQCloudy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelSendGroupMsgEvent
{
    public class IfTextMsg : IEventHandler<Dto.EventBus.ChannelSendGroupMsgEvent<string>>
    {
        IIMHepler imservice = new IMHelper();
        ConversationRoomService roomService = new ConversationRoomService();

        public bool Handle(Dto.EventBus.ChannelSendGroupMsgEvent<string> evt)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(evt.Msg))
                {
                    return true;
                }
                else
                {
                    return imservice.SendGroupTextMsg(evt.ChannelID, evt.FromAccount, evt.Msg);
                }
            }
            catch (InvalidGroupException)
            {
                //自动修正房间不存在的问题
                using (MQChannel mq = new MQChannel())
                {
                    var room = roomService.GetChannelInfo(evt.ChannelID);
                    if (roomService.UpdateRoomEable(evt.ChannelID, false))
                    {
                        mq.Publish(new Dto.EventBus.ChannelCreateEvent()
                        {

                            ChannelID = evt.ChannelID,
                            ServiceID = room.ServiceID,
                            ServiceType = room.ServiceType,
                        });
                    }

                    return false;
                }
            }
            catch (Exception Ex)
            {
                LogHelper.DefaultLogger.Error(Ex);
                return false;
            }

        }
    }
}
