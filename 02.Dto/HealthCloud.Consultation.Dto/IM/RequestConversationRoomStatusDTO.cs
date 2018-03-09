using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.IM
{
    /// <summary>
    /// 房间状态
    /// </summary>
    public class RequestConversationRoomStatusDTO
    {
        [Required]
        /// <summary>
        /// 房间ID
        /// </summary>
        public string ChannelID { get; set; }

        [Required]
        /// <summary>
        /// 房间状态
        /// </summary>
        public EnumRoomState State { get; set; }


        /// <summary>
        /// 服务时长（单位：秒）
        /// </summary>
        public int Duration { get; set; }

        public string ServiceID { get; set; }

        public EnumDoctorServiceType ServiceType { get; set; }

        public int TotalTime { get; set; }

        public EnumRoomChargingState ChargingState { get; set; }

        /// <summary>
        /// 禁用互通性（意味着采用P2p通信）
        /// </summary>
        public bool DisableWebSdkInteroperability { get; set; }
    }

   
}
