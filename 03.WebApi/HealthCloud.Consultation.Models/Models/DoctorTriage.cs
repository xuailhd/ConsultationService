using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;


namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// ����ҽ��
    /// </summary>
    public partial class DoctorTriage : AuditableEntity
    {
        /// <summary>
        /// ԤԼID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        [Key, Required]
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// ����ҽ��ID
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string TriageDoctorID { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string TriageDoctorName { get; set; }

        /// <summary>
        /// ����״̬��0�ޣ�1�����2�����У�3�ѷ��
        /// </summary>
        public EnumTriageStatus TriageStatus { get; set; }

        /// <summary>
        /// �Ƿ�Ҫ��������ϵͳ
        /// </summary>
        public bool IsToGuidance { get; set; }

    }
}
