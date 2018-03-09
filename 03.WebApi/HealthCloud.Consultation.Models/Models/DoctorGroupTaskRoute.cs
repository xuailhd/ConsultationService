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
    /// 医生分组  路由
    /// </summary>
    public class DoctorGroupTaskRoute
    {
        [Key, Required]
        [Column(TypeName = "varchar",Order =0)]
        [MaxLength(32)]
        public string DoctorGroupID { get; set; }

        [Key, Required]
        [Column(Order =1)]
        public int UserLevel { get; set; }
    }
}
