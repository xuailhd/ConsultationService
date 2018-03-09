using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Json;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Services.DrKang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelNewMsgEvent
{
    /// <summary>
    /// 呼叫康博士回答
    /// </summary>
    public class IfReturnVisitCallDrKangAnswer : IEventHandler<Dto.EventBus.ChannelNewMsgEvent>
    {
        public class Msg
        {

            public string MsgType { get; set; }

            public Dictionary<string, string> MsgContent { get; set; }

        }


        DrKangService drKangService = new DrKangService();
        ConversationRoomService roomService = new ConversationRoomService();


        bool IsEnable()
        {

            //已经启用了康博士
            if (!string.IsNullOrEmpty(HealthCloud.Consultation.Services.DrKang.Configuration.Config.drKangEnable) &&
                             (HealthCloud.Consultation.Services.DrKang.Configuration.Config.drKangEnable == "1" ||
                             HealthCloud.Consultation.Services.DrKang.Configuration.Config.drKangEnable.ToUpper() == bool.TrueString.ToUpper())
                             )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Handle(Dto.EventBus.ChannelNewMsgEvent evt)
        {
            if (evt == null)
                return true;

            //是通过客户端发送的
            if (evt.OptPlatform != "RESTAPI" && IsEnable())
            {
                #region 检查当前咨询中康博士回答情况，判断是否还需要继续使用康博士
                var cacheKey_Channel_DrKangState = new StringCacheKey(StringCacheKeyType.Channel_DrKangState, evt.ChannelID.ToString());
                var Channel_DrKangState = cacheKey_Channel_DrKangState.FromCache<string>();

                switch (Channel_DrKangState)
                {
                    //问答结束，没有匹配的疾病            
                    case "nullMatchDisease":
                    //问答结束，已有明确诊断
                    case "diagnosis":
                    //无法响应回复
                    case "nullMatchResponse":
                    //禁用(医生已回复)
                    case "disabled":
                    //出现异常
                    case "exception":
                        return true;
                }
                #endregion

                //文字内容才识别
                if (evt.Messages.Length > 0 && evt.Messages[0].MessageContent.Contains("\"MsgType\":\"TIMTextElem\""))
                {
                    var room = roomService.GetChannelInfo(evt.ChannelID);

                    //医生未接诊
                    if (room != null && room.RoomState == EnumRoomState.AlreadyVisit)
                    {
                        //  { "MsgContent":{ "Text":"头疼"},"MsgType":"TIMTextElem"}
                        var msg = JsonHelper.FromJson<Msg>(evt.Messages[0].MessageContent);

                        if (msg != null && msg.MsgType == "TIMTextElem" && msg.MsgContent != null)
                        {
                            var text = msg.MsgContent["Text"];

                           
                        }
                    }
                    else
                    {
                        //医生已经回答
                        "disabled".ToCache(cacheKey_Channel_DrKangState, TimeSpan.FromHours(24));
                    }
                }
            }

            return true;
        }
    }


}
