using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Common.Config
{
    /// <summary>
    /// 会诊配制
    /// </summary>
    public class Consultation 
    {
        /// <summary>
        /// 平台会诊服务价格
        /// </summary>
        public string PlatServicePrice { get; set; }

        public decimal ServicePrice
        {
            get
            {
                decimal i = 0;
                decimal.TryParse(PlatServicePrice, out i);
                return i;
            }
        }

        public string ConsultationApiUrl { get; set; }

        public string MsgConsulPaidNoticeTemlateId { get; set; }
        public string MsgConsulOnlinePayNoticeTemlateId { get; set; }
        public string MsgConsulCreateNoticeTemlateId { get; set; }

        public string MsgConsulCreateNoticeTemlateContent { get; set; }

        public string MsgConsulOnlinePayNoticeTemlateContent { get; set; }

        public string MsgConsulPaidNoticeTemlateContent { get; set; }

    }
}
