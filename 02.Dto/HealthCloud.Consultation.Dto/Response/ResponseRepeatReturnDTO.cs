using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseRepeatReturnDTO
    {
        /// <summary>
        ///外部订单号码
        /// </summary>
        public string OrderOutID { get; set; }

        /// <summary>
        /// 商户网站唯一订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public EnumOPDState OPDState { get; set; }

        public string ChannelID { get; set; }

        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 是否可以取消
        /// </summary>
        public bool Cancelable { get; set; }
    }
}
