using HealthCloud.Common.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.EventBus
{
    /// <summary>
    /// 医生回访事件
    /// </summary>
    public  class DoctorReturnVisitEvent: BaseEvent, IEvent
    {
        public string ReturnVisitContent { get; set; }
        public string ReturnVisitID { get; set; }
        public string ServiceID { get; set; }
    }
}
