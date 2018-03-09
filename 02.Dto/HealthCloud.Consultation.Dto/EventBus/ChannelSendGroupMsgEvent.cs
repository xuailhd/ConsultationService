using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.EventBus
{
    public class ChannelSendGroupMsgEvent<TMsg>: BaseEvent, IEvent
    {
        public string ChannelID { get; set; }

        public int FromAccount { get; set; }

        public TMsg Msg { get; set; }
    }
}
