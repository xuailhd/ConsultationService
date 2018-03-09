using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HealthCloud.Consultation.Models
{
    
    /// <summary>
    /// 检查结果
    /// </summary>
    public partial class UserInspectResult : AuditableEntity
    {
        /// <summary>
        /// 检查结果ID
        /// </summary>
        [Key,Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string InspectResultID { get; set; }

        /// <summary>
        /// 成员ID
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string MemberID { get; set; }

        /// <summary>
        /// 检查类型
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public string InspectType { get; set; }

        /// <summary>
        /// 检查部位
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string InspectPoint { get; set; }

        /// <summary>
        /// 检查日期
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(16)]
        public string InspectDate { get; set; }

        /// <summary>
        /// 医生建议
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(1024)]
        public string DoctorSuggest { get; set; }

        /// <summary>
        /// 上传文件名称
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(256)]
        public string FileUploadName { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(256)]
        public string FileName { get; set; }

        /// <summary>
        /// 案例ID
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(64)]
        public string CaseID { get; set; }

        /// <summary>
        /// DCM文件解析出来的字段
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(256)]
        public string StudyID { get; set; }

        /// <summary>
        /// DCM文件解析出来的字段
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(256)]
        public string StuUID { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(50)]
        public string ImgMD5 { get; set; }

    }
}
