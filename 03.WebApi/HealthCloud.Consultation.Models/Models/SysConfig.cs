using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Models
{

    public class SysConfig
    { 
        /// <summary>
        /// 配置节Key
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar")]
        
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置节Value
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(512)]
        public string ConfigValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(512)]
        public string Remark { get; set; }

    }
}
