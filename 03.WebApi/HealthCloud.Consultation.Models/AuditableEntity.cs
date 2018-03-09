using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{

    public class AuditableEntity
    {

        public AuditableEntity()
        {
            CreateUserID = string.Empty;
            CreateTime = DateTime.Now;
            ModifyUserID = string.Empty;
            ModifyTime = DateTime.Now;
            IsDeleted = false;
        }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [Column(TypeName ="varchar")]
        public string CreateUserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [Column(TypeName = "nvarchar")]
        public string ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTimeOffset? ModifyTime { get; set; }

        /// <summary>
        /// 删除用户ID
        /// </summary>
        [Column(TypeName = "nvarchar")]
        public string DeleteUserID { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTimeOffset? DeleteTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Column(TypeName ="bit")]
        public bool IsDeleted { get; set; }
    }
}
