using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;


namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 分诊医生
    /// </summary>
    public partial class DoctorTriage : AuditableEntity
    {
        /// <summary>
        /// 预约ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        [Key, Required]
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 分诊医生ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string TriageDoctorID { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string TriageDoctorName { get; set; }

        /// <summary>
        /// 分诊状态（0无，1待分诊，2分诊中，3已分诊）
        /// </summary>
        public EnumTriageStatus TriageStatus { get; set; }

        /// <summary>
        /// 是否要经过导诊系统
        /// </summary>
        public bool IsToGuidance { get; set; }

    }
}
