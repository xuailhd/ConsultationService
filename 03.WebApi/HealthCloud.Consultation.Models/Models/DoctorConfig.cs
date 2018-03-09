using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 医生配置信息表
    /// </summary>
    public class DoctorConfig: AuditableEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ConfigID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string DoctorID { get; set; }

        /// <summary>
        /// 配置类型
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumDoctorConfigType ConfigType { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(200)]
        public string ConfigContent { get; set; }
    }
}
