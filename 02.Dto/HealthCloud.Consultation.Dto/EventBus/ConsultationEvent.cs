using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.EventBus
{
    public class ConsultationOperationEvent : BaseEvent, IEvent
    {

        public string ConsultationID { get; set; }

        public EnumConsultationOperationType OperationType { get; set; }

        public ResponseConsultationLogDTO OperationLog { get; set; }

        public EnumConsultationProgress OperationingConsultationProgress { get; set; }

        public string CurrentOperatorUserID { get; set; }

        public string TradeNo { get; set; }
    }
}
