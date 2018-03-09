using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.EventBus
{
    public class ChannelStateChangedEvent : BaseEvent, IEvent
    {
        public string ChannelID { get; set; }

        public string FromUserID { get; set; }

        public EnumUserType UserType { get; set; }

        public int FromUseridentifier { get; set; }

        public string FromPlatform { get; set; }

        public EnumRoomState State { get; set; }

        /// <summary>
        /// 期望状态
        /// </summary>
        public EnumRoomState? ExpectedState { get; set; }

        public bool DisableWebSdkInteroperability { get; set; }

        public bool EnterRoomSendNotice { get; set; } = true;
    }
}
