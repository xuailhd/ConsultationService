﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 看诊房间操作日志
    /// </summary>
    public class ConversationRoomLog
    {
        [Key, Required]
        [Column(TypeName = "nvarchar")]
        public string ConversationRoomLogID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string ConversationRoomID { get; set; }


        /// <summary>
        /// 操作人ID
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string OperationUserID { get; set; }

        /// <summary>
        /// 操作用户名称
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string OperatorUserName { get; set;}
        
        /// <summary>
        /// 操作类型
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        
        public string OperatorType { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Required]
        public DateTimeOffset OperationTime { get; set; }

        /// <summary>
        /// 操作描述
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(200)]
        public string OperationDesc { get; set; }


        /// <summary>
        /// 操作说明
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(200)]
        public string OperationRemark { get; set; }
    }
}
