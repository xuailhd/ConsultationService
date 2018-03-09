using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 实时通讯用户唯一标识
    /// </summary>
    public class ConversationIMUid : AuditableEntity
    {
        public ConversationIMUid()
        {
            this.Enable = false;
        }

        /// <summary>
        /// 用户唯一标识（必须是Int类型）
        /// </summary>
        [Key]
        [Required]
        public int Identifier { get; set; }


        /// <summary>
        /// 外部用户编号
        /// </summary>
        [Required, Column(TypeName = "nvarchar")]
        public string UserID { get; set; }

        /// <summary>
        /// 是否有效  云通信注册成功后更新
        /// </summary>
        public bool Enable { get; set; }
    }
}
