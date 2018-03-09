using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{
    /// <summary>
    /// 看诊房间表
    /// </summary>
    public class ConversationRoom : AuditableEntity
    {
        public ConversationRoom()
        {
            this.RoomType = EnumRoomType.Group;
            this.TriageID =long.MaxValue;
            this.Duration = -1;
            this.ChargingSeq = 0;
            this.ChargingState = EnumRoomChargingState.Stoped;
            this.ChargingInterval = 15;
            this.Secret = "";
            this.DisableWebSdkInteroperability = false;
            this.ChargingTime = new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// 会话房间ID
        /// </summary>
        [Key,Required]
        [Column(TypeName = "nvarchar")]
        public string ConversationRoomID { get; set; }

        /// <summary>
        /// 业务ID(关联外部业务ID)
        /// </summary>      
        [Column(TypeName = "nvarchar")]
        [Required]
        public string ServiceID { get; set; }


        /// <summary>
        /// 业务类型
        /// </summary>
        [Required]
        public EnumDoctorServiceType ServiceType { get; set; }
        
        /// <summary>
        /// 房间编号
        /// </summary>
        //public int ChannelID { get; set; }

        /// <summary>
        /// 房间密码
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [Required]
        public string Secret { get; set; }

        /// <summary>
        /// 房间状态
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumRoomState RoomState { get; set; }


        /// <summary>
        /// 看诊开始时间
        /// </summary>
        [Required]
        public DateTimeOffset BeginTime { get; set; }

        /// <summary>
        /// 看诊结束时间
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// 看诊总时长(单位：秒)
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public int TotalTime { get; set; }

        /// <summary>
        /// 服务时长（单位：秒，小于0不限制）
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public int Duration { get; set; }

        /// <summary>
        /// 房间是否有效
        /// </summary>
        [Required]
        public bool Enable { get; set; }

        /// <summary>
        /// 是否禁用SDK(兼容插件非插件版)
        /// </summary>
        [Required]
        public bool DisableWebSdkInteroperability { get; set;}

        /// <summary>
        /// 房间是否已关闭（关闭后无法设置房间状态）
        /// </summary>
        [Required]
        public bool Close { get; set; }

        /// <summary>
        /// 房间类型
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumRoomType RoomType { get; set; }

        /// <summary>
        /// 分诊编号（线下看诊通过分诊编号来叫号）
        /// </summary>
        [Required]
        public long TriageID { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Required]
        public int Priority { get; set; }

        /// <summary>
        /// 计费状态
        /// </summary>
        [Required]
        public EnumRoomChargingState ChargingState { get; set; }


        /// <summary>
        /// 计费时钟序号
        /// </summary>
        [Required]
        public int ChargingSeq { get; set; }

        /// <summary>
        /// 计费最后时间
        /// </summary>
        public DateTimeOffset ChargingTime { get; set; }

        [Required]
        /// <summary>
        /// 计费时钟周期
        /// </summary>
        public int ChargingInterval { get; set; }

    }
}
