using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 我的会诊
    /// </summary>
    public class ResponseDoctorConsultationInviteDTO
    {
        /// <summary>
        /// 会诊邀请编号
        /// </summary>
        public string ConsultationInviteID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 会诊单ID
        /// </summary>
        public string ConsultationID { get; set; }

        /// <summary>
        /// 会诊医生意见
        /// </summary>
        public string Opinion { get; set; }

        /// <summary>
        /// 会诊医生金额
        /// </summary>
        public decimal Amount { get; set; }

        public DateTimeOffset CreateTime { get; set; }

        public DateTimeOffset? ModifyTime { get; set; }

        /// <summary>
        /// 是否是主诊医生
        /// </summary>
        public bool IsConsult { get; set; }
        /// <summary>
        /// 医生
        /// </summary>
        public ResponseDoctorInfoDTO Doctor { get; set; }

    }

 


}
