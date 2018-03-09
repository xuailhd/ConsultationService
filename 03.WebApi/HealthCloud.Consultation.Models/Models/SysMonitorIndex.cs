using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 监控用户看诊指标
    /// </summary>
    public class SysMonitorIndex
    {
        public SysMonitorIndex()
        {
        }

        /// <summary>
        /// 外部编号
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar",Order =2)]
        [MaxLength(32)]
        public string OutID { get; set; }

        /// <summary>
        /// 指标分类
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar",Order =0)]
        [MaxLength(50)]
        public string Category { get; set; }



        /// <summary>
        /// 指标名称
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar",Order =1)]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 指标数据
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(1000)]
        public string Value { get; set;}


        [Required]
        public DateTimeOffset ModifyTime { get; set; }
    }
}
