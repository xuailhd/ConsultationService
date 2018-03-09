using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 房间状态
    /// </summary>
    public class RequestConversationRoomTriageDTO
    {
        /// <summary>
        /// 房间号
        /// </summary>
        [Required]
        public string ChannelID { get; set; }

        /// <summary>
        /// 当前操作医生ID
        /// </summary>
        [Required]
        public string CurrentOperatorDoctorID { get; set; }

    }
}
