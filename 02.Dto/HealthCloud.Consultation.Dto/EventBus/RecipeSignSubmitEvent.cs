using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.EventBus
{

    public class RecipeSignSubmitEvent : BaseEvent, IEvent
    {
        /// <summary>
        /// 业务编号
        /// </summary>
        public string ServiceID { get; set; }

    }
}
