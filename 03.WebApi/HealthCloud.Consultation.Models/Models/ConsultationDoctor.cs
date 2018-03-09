using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 会诊医生
    /// </summary>
    public class ConsultationDoctor : AuditableEntity
    {

        /// <summary>
        /// 会诊医生ID
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConsultationDoctorID { get; set; }

        /// <summary>
        /// 会诊ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConsultationID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string DoctorID { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string DoctorName { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(255)]
        public string PhotoUrl { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string DepartmentName { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string HospitalName { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(20)]
        public string Title { get; set; }

        /// <summary>
        /// 会诊医生金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 会诊意见
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(4000)]
        public string Opinion { get; set; }

        /// <summary>
        /// 治疗方案
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(4000)]
        public string Perscription { get; set; }

        /// <summary>
        /// 是否主诊医生
        /// </summary>
        [Column(TypeName = "bit")]
        public bool IsAttending { get; set; }

    }
}
