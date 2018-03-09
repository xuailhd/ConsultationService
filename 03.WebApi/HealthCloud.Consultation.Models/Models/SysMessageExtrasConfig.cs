using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 消息附加参数配制
    /// </summary>
    public class SysMessageExtrasConfig : AuditableEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, Required]
        [Column(TypeName = "varchar")]
        [MaxLength(32)]
        public string ExtrasConfigID { get; set; }

        /// <summary>
        /// 终端类型
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumTerminalType TerminalType { get; set; }

        /// <summary>
        /// 消息分类
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumNoticeSecondType MsgType { get; set; }

        /// <summary>
        /// 通知模板
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(4000)]
        public string MsgTitle { get; set; }

        /// <summary>
        /// 跳转页面
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(512)]
        [Required]
        public string PageUrl { get; set; }

        /// <summary>
        /// //页面类型（HTML/Native）
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(20)]
        public string PageType { get; set; }

        /// <summary>
        /// 打开目标(_Blank/_Parent/_Self/_Top)
        /// </summary>
        [Column(TypeName = "varchar")]
        [MaxLength(20)]
        public string PageTarget { get; set; }

    }
}
