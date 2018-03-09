using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{

    /// <summary>
    /// 房间状态
    /// </summary>
    public class RequestConversationRoomRenewUpgradeDTO
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 房间状态
        /// </summary>
        public EnumRoomState State { get; set; }


        public string ServiceID { get; set; }

        public EnumDoctorServiceType ServiceType { get; set; }

        /// <summary>
        /// 续费订单编号
        /// </summary>
        public string RenewOrderNo { get; set; }

    }
}
