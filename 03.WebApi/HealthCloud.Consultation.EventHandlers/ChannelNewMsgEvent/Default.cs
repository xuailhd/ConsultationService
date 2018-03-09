using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelNewMsgEvent
{
    /// <summary>
    /// 房间收到新消息
    /// 触发条件：云通信回调
    /// 前置条件：无
    /// 后置条件：消息记录写入数据库
    /// </summary>
    public class DefaultHandler : IEventHandler<Dto.EventBus.ChannelNewMsgEvent>
    {
        public bool Handle(Dto.EventBus.ChannelNewMsgEvent evt)
        {
            try
            {
                if (evt == null)
                    return true;

                ConversationMessageService bll = new ConversationMessageService();

                if(evt.Messages!=null && evt.Messages.Length>0)
                {
                    List<ResponseConversationMsgDTO> msgs = new List<ResponseConversationMsgDTO>();
                    for (int i= 0; i < evt.Messages.Length; i ++)
                    {
                        if (bll.Single(evt.Messages[i].ConversationMessageID) == null)
                        {
                            msgs.Add(evt.Messages[i]);
                        }
                    }

                    if (msgs.Count > 0)
                    {
                        if (bll.Insert(msgs))
                        {
                            return true;

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception E)
            {
                LogHelper.DefaultLogger.Error(E);
            }

            return false;
        }
    }


}
