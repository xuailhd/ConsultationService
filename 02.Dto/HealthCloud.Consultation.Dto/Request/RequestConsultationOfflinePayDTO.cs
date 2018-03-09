using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{

    public class RequestConsultationOfflinePayDTO
    {
        public string ConsultationID { get; set; }

        public decimal Amount  { get; set; }

        public string CurrentOperatorUserID { get; set; }

        public string CurrentOperatorUserName { get; set; }

        public string OrgID { get; set; }

        public string TradeNo { get; set; }

        public List<RequestUserFileDTO> Files { get; set; }
    }
}
