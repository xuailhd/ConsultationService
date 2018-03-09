using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Response;
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
    public class IfImageMsg : IEventHandler<Dto.EventBus.ChannelSendGroupMsgEvent<ResponseUserFileDTO>>
    {
        IIMHepler imservice = new IMHelper();
        ConversationRoomService roomService = new ConversationRoomService();
        IMGStore storeConfig = null;

        public IfImageMsg()
        {
            //文件存储配置
            storeConfig = SysConfigService.Get<IMGStore>();
        }

        public bool Handle(Dto.EventBus.ChannelSendGroupMsgEvent<ResponseUserFileDTO> evt)
        {
            try
            {
                //发送图片消息
                if (evt.Msg.FileType == 0)
                {
                    return imservice.SendGroupImageMsg(evt.ChannelID, evt.FromAccount, evt.Msg.FileID, evt.Msg.FileUrl);
                }
                //发送图片消息
                else if (evt.Msg.FileType == 1)
                {
                    return imservice.SendGroupFileMsg(evt.ChannelID, evt.FromAccount, evt.Msg.FileID, evt.Msg.FileSize, evt.Msg.FileName);
                }
                //发送音频消息
                //else if (evt.Msg.FileType == 2)
                //{
                //    using (var filestream = KMEHosp.Common.Storage.Manager.Instance.OpenFile("Audios", evt.Msg.FileUrl))
                //    {
                //        Task.WaitAll(filestream);
                  
                //        var Second = Convert.ToInt32(KMEHosp.Common.Utility.AudioHelper.TotalSeconds(filestream.Result));
                //        return imservice.SendGroupAudioMsg(evt.ChannelID, evt.FromAccount, evt.Msg.FileID, evt.Msg.FileSize, Second);
                     
                //    }
                //}
                else
                {
                    return true;
                }
            }
            catch (InvalidGroupException)
            {
                //自动修正房间不存在的问题
                using (MQChannel mq = new MQChannel())
                {
                    var room = roomService.GetChannelInfo(evt.ChannelID);
                    room.Enable = false;
                    if (roomService.UpdateRoomEable(evt.ChannelID,false))
                    {
                        mq.Publish(new ChannelCreateEvent()
                        {
                            ChannelID = evt.ChannelID,
                            ServiceID = room.ServiceID,
                            ServiceType = room.ServiceType,
                        });
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex);
                return false;
            }
        }
    }
}
