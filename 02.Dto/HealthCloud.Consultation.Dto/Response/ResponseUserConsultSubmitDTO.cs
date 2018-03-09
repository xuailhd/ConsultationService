using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 用户咨询提交时返回结果
    /// </summary>
    public class ResponseUserConsultSubmitDTO
    {
        /// <summary>
        /// 咨询编号
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public string ActionStatus { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNO { get; set; }


        public EnumOPDState OPDState { get; set; }

        public string MemberID { get; set; }

        /// <summary>
        /// 房间频道ID
        /// </summary>
        public int ChannelID { get; set; }
    }
}