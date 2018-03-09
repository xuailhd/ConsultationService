using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestConsultationSearchDTO : RequestSearchCondition
    {
        public string ConsultationNo { get; set; }
        public string OrderNo { get; set; }
        public string CurrentOperatorDoctorID { get; set; }
        public string SpecialtyDoctorID { get; set; }
        public string CurrentOperatorUserID { get; set; }
        public EnumConsultationSource? ConsultationSource { get; set; }

        /// <summary>
        /// 成员IDs
        /// </summary>
        public List<string> MemberIds { get; set; }
        /// <summary>
        /// 多选查询
        /// </summary>
        public List<EnumConsultationProgress> ConsultationProgresses { get; set; }

        public EnumOPDState? OPDState { get; set; }

        /// <summary>
        /// 导诊平台查询订单状态
        /// </summary>
        public int? GOrderState { get; set; }

        /// <summary>
        /// 查询我是主诊和邀请的所有会诊单
        /// </summary>
        public bool IsSelectAll { get; set; }

        public DateTimeOffset? BeginDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        #region 导诊台条件查询
        public string MemberName { get; set; }
        public string Mobile { get; set; }
        public string SpecialtyDoctorName { get; set; }
        public string DoctorName { get; set; }

        public string MemberID { get; set; }
        #endregion

        public bool OrderByStartTimeReal { get; set; }
    }
}
