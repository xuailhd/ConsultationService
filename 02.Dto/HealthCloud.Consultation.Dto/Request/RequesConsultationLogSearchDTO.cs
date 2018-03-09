using HealthCloud.Consultation.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequesConsultationLogSearchDTO : RequestSearchCondition
    {
        public string ConsultationID { get; set; }

    }
}
