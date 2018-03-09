using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 线下挂号记录表
    /// </summary>
    public class UserRegistration : AuditableEntity
    {
        public UserRegistration()
        {
            this.TriageID = "";
            this.CaseNo = "";
        }
        /// <summary>
        /// 预约编号
        /// </summary>
        [Key, Required]
        [Column(TypeName = "nvarchar")]
        
        public string RegistrationID
        { get; set; }            

        /// <summary>
        /// 外部号源ID
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string OutID { get; set; }

        /// <summary>
        /// 医院编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string HospitalID { get; set; }

        /// <summary>
        /// 科室编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string DepartmentID { get; set; }

        /// <summary>
        /// 医生编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string DoctorID { get; set; }

        /// <summary>
        /// 患者编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string PatientID { get; set; }


        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required]
        public int Sex { get; set; }


        /// <summary>
        /// 电话
        /// </summary>
        [Column(TypeName = "nvarchar")]
        public string Mobile { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(200)]
        public string Email { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(10)]
        public string Birth { get; set; }

        /// <summary>
        /// 证件类型（0-身份证）
        /// </summary>
        [Column(TypeName = "int")]
        public EnumUserCardType IDType { get; set; }


        /// <summary>
        /// 患者类型
        /// </summary>
        public string PatientType
        { get; set; }

        /// <summary>
        /// 挂号类型 1：普通挂号；2：预约挂号；3：转诊挂号；4：体检系统默认挂号     
        /// </summary>
        public EnumRegistrationType RegType { get; set; }

        /// <summary>
        /// 就诊类型 ，1：普通；2：专家  
        /// </summary>
        public EnumRegistrationMedicalType MedicalType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Column(TypeName = "nvarchar")]
        public string IDNumber { get; set; }

        /// <summary>
        /// 分诊编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string TriageID { get; set; }


        /// <summary>
        /// 诊疗卡
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string MedicalCardID { get; set; }


        /// <summary>
        /// 就诊日期
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string OPDDate { get; set; }


        /// <summary>
        /// 看诊开始时间
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(10)]
        public string OPDBeginTime { get; set; }

        /// <summary>
        /// 看诊结束时间
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(10)]
        public string OPDEndTime { get; set; }

        /// <summary>
        /// 预约编号
        /// </summary>
        [Required]
        public string OPDRegisterID
        { get; set; }


        /// <summary>
        /// 就诊编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string CaseNo { get; set; }


        /// <summary>
        /// 科室编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string OutDepartmentID { get; set; }

        /// <summary>
        /// 医生编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string OutDoctorID { get; set; }

    }
}
