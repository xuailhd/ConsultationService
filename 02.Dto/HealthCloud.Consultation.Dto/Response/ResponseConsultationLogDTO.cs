using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseConsultationLogDTO
    {
        /// <summary>
        /// 会诊日志ID
        /// </summary>
        public string ConsultationLogID { get; set; }

        /// <summary>
        /// 会诊ID
        /// </summary>
        public string ConsultationID { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public EnumConsultationOperationType OperationType { get; set; }

        /// <summary>
        /// 操作后会诊进度(状态)
        /// </summary>
        public EnumConsultationProgress ConsultationProgress { get; set; }

        /// <summary>
        /// 操作后订单状态
        /// </summary>
        public EnumOPDState OPDState { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 操作来源
        /// </summary>
        public string OrgID { get; set; }

        public string CreateUserID { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperationUserName { get; set; }

        public DateTimeOffset CreateTime { get; set; }
    }
}
