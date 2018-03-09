using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.EventBus
{
    public class UserNoticeEvent: BaseEvent, IEvent
    {
        public string FromUserID
        { get; set; }

        public EnumUserType FromUserType { get; set; }

        public EnumNoticeSecondType NoticeType { get; set; }

        public EnumDoctorServiceType ServiceType { get; set; }

        public string ServiceID { get; set; }
    }
}
