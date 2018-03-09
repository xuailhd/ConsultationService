using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 会诊建议和总结
    /// </summary>
    public class RequestConsultationCommentsDTO
    {
        public string ConsultationDoctorID { get; set; }
        public string ConsultationID { get; set; }
        public string CurrentOperatorUserID { get; set; }

        public string CurrentOperatorUserName { get; set; }

        /// <summary>
        /// 会诊意见
        /// </summary>
        public string Opinion { get; set; }

        /// <summary>
        /// 治疗方案
        /// </summary>
        public string Perscription { get; set; }

        public string OrgID { get; set; }
    }
}
