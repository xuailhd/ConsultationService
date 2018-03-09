using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Dto.Response;

namespace HealthCloud.Consultation.Dto.EventBus
{
    /// <summary>
    /// 频道新消息事件
    /// </summary>
    public class ChannelNewMsgEvent : BaseEvent, IEvent
    {

        public string ChannelID { get; set; }

        public EnumDoctorServiceType ServiceType { get; set; }

        public string ServiceID { get; set; }

        public string FromAccount { get; set;}

        public ResponseConversationMsgDTO[] Messages { get; set; }

        public string OptPlatform { get; set; }
    }
}
