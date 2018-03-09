using HealthCloud.Common.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.EventBus
{
    public class ChannelTriageChangeEvent: BaseEvent, IEvent
    {
        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorID { get; set; }
        
        /// <summary>
        /// 频道编号
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 分诊编号
        /// </summary>
        public string TriageID { get; set; }

    }
}
