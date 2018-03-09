using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.EventBus
{
    public class ChannelCreatedEvent: BaseEvent, IEvent
    {
        public string ServiceID { get; set; }

        public EnumDoctorServiceType ServiceType { get; set; }

        public string ChannelID { get; set; }

    }
}
