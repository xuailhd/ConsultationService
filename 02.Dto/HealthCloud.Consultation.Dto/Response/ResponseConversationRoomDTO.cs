using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 会话房间
    /// </summary>
    public class ResponseConversationRoomDTO
    {
        public ResponseConversationRoomDTO()
        {
            this.DisableWebSdkInteroperability = false;
        }

        /// <summary>
        /// 会话房间ID
        /// </summary>
        [Required]
        public string ConversationRoomID { get; set; }

        /// <summary>
        /// 业务ID(关联外部业务ID)
        /// </summary>
        [Required]
        public string ServiceID { get; set; }

        public string OrderNo { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctorID { get; set; }

        public string DoctorName { get; set; }

        public string UserID { get; set; }

        public string MemberID { get; set; }

        /// <summary>
        /// 成员姓名
        /// </summary>
        public string MemberName { get; set; }


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
        public string Secret { get; set; }

        /// <summary>
        /// 房间状态
        /// </summary>
        [Required]
        public EnumRoomState RoomState { get; set; }

        public string RoomStateName { get; set; }


        /// <summary>
        /// 看诊开始时间
        /// </summary>
        [Required]
        public DateTimeOffset BeginTime { get; set; }

        /// <summary>
        /// 看诊结束时间
        /// </summary>
        public DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// 看诊总时长
        /// </summary>
        [Required]
        public int TotalTime { get; set; }

        public bool Enable { get; set; }

        /// <summary>
        /// 房间是否已关闭（关闭状态无法设置房间状态）
        /// </summary>
        public bool Close { get; set; }

        public int Duration { get; set; }

        /// <summary>
        /// 计费状态
        /// </summary>
        public EnumRoomChargingState ChargingState { get; set; }

        /// <summary>
        /// 计费时钟序号
        /// </summary>
        public int ChargingSeq { get; set; }

        /// <summary>
        /// 计费时钟周期
        /// </summary>
        public int ChargingInterval { get; set; }

        /// <summary>
        /// 计费最后时间
        /// </summary>
        public DateTimeOffset ChargingTime { get; set; }

        public EnumRoomType RoomType { get; set; }

        public long TriageID { get; set; }

        public int Priority { get; set; }

        public DateTimeOffset ModifyTime { get; set; }

        public bool DisableWebSdkInteroperability { get; set;}

    }
    
}
