using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 会诊日志
    /// </summary>
    public class ConsultationLog : AuditableEntity
    {

        /// <summary>
        /// 会诊日志ID
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConsultationLogID { get; set; }

        /// <summary>
        /// 会诊ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConsultationID { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumConsultationOperationType OperationType { get; set; }

        /// <summary>
        /// 操作后会诊进度(状态)
        /// </summary>
        [Column(TypeName = "int")]
        public EnumConsultationProgress ConsultationProgress { get; set; }

        /// <summary>
        /// 操作后订单状态
        /// </summary>
        [Column(TypeName = "int")]
        public EnumOPDState OPDState { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(4000)]
        public string Remark { get; set; }

        /// <summary>
        /// 操作来源
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string OrgID { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string OperationUserName { get; set; }

    }
}
