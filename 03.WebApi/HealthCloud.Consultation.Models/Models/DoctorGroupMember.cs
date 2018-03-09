using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Models
{
    public class DoctorGroupMember: AuditableEntity
    {
        [Key, Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string GroupMemberID { get; set; }

        /// <summary>
        /// 医生分组编号
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string DoctorGroupID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string DoctorID { get; set; }

    }
}
