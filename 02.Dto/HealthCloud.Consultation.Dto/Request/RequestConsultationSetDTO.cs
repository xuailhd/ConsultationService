using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestConsultationSetDTO
    {
        /// <summary>
        /// 会诊ID
        /// </summary>
        [Required]
        public string ConsultationID
        {
            get; set;
        }

        /// <summary>
        /// 会诊价格
        /// </summary>
        public decimal? Amount { get; set; }
        public string OrgId { get; set; }

        public string CurrentOperatorUserID { get; set; }
        public string CurrentOperationUserName { get; set; }
    }
}
