using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 远程会诊
    /// </summary>
    public class RemoteConsultation : AuditableEntity
    {
        /// <summary>
        /// 会诊ID
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConsultationID { get; set; }

        /// <summary>
        /// 成员ID
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string MemberID { get; set; }

        ///// <summary>
        ///// 会诊主题(主诉)
        ///// </summary>
        //[Column(TypeName = "nvarchar")]
        //[MaxLength(4000)]
        //public string ConsultationSubject { get; set; }

        /// <summary>
        /// 会诊要求
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(4000)]
        public string Requirement { get; set; }

        /// <summary>
        /// 会诊目的
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(4000)]
        public string Purpose { get; set; }

        ///// <summary>
        ///// 病情描述
        ///// </summary>
        //[Column(TypeName = "nvarchar")]
        //[MaxLength(4000)]
        //public string Description { get; set; }

        ///// <summary>
        ///// 初步诊断
        ///// </summary>
        //[Column(TypeName = "nvarchar")]
        //[MaxLength(4000)]
        //public string PreliminaryDiagnosis { get; set; }

        /// <summary>
        /// 会诊开始时间
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// 会诊完成时间
        /// </summary>
        public DateTimeOffset? FinishTime { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTimeOffset? StartTimeReal { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTimeOffset? FinishTimeReal { get; set; }

        /// <summary>
        /// 会诊号
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConsultationNo { get; set; }

        /// <summary>
        /// 押金
        /// </summary>
        [Required]
        [Column(TypeName = "decimal")]
        public decimal Deposit { get; set; }

        /// <summary>
        ///  会诊费用
        /// </summary>
        [Required]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 会诊进度(状态)
        /// </summary>
        [Column(TypeName = "int")]
        public EnumConsultationProgress ConsultationProgress { get; set; }

        /// <summary>
        /// 会诊来源
        /// </summary>
        [Column(TypeName = "int")]
        public EnumConsultationSource ConsultationSource { get; set; }

        /// <summary>
        /// 预约ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 会诊地点
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(100)]
        public string Address { get; set; }

    }
}
