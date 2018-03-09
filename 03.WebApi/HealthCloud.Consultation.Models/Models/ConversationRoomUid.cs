using HealthCloud.Consultation.Enums;
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
    /// 会话房间 成员表
    /// </summary>
    public class ConversationRoomUid : AuditableEntity
    {
        public ConversationRoomUid()
        {
            PhotoUrl = "";
        }


        [Key, Required, Column(Order = 0)]
        /// <summary>
        /// 房间编号
        /// </summary>
        public string ConversationRoomID { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        [Key, Required,Column(Order=1)]
        public int  Identifier { get; set; }

        public EnumUserType UserType { get; set; }

        /// <summary>
        /// 用户成员编号
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string UserMemberID { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string UserID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(64)]
        public string UserCNName { get; set; }


        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(64)]
        public string UserENName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public EnumUserGender Gender { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(200)]
        public string PhotoUrl { get; set; }
    }
}
