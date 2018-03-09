using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 看诊房间时长续费日志
    /// </summary>
    public class ConversationRoomUpgrade : AuditableEntity
    {
        /// <summary>
        /// 业务ID(关联外部业务ID)
        /// </summary>
        [Key, Required]
        [Column(TypeName = "nvarchar",Order =0)]
        public string ServiceID { get; set; }

        /// <summary>
        /// 原始订单号d
        /// </summary>
        [Key, Column(TypeName = "nvarchar", Order =1)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 续费订单号
        /// </summary>
        [Column(TypeName = "nvarchar",Order = 2)]
        [Key, Required]
        public string NewUpgradeOrderNo { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 是否已经发送了服务时长消息
        /// </summary>
        [Required]
        public bool IsSendDurationMsg { get; set; }

        /// <summary>
        /// 是否已经发送了到期提醒
        /// </summary>
        [Required]
        public bool IsSendExpireMsg { get; set; }
    }
}
