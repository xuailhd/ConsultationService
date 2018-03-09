using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 统计获取统计信息
    /// </summary>
    public class RequestGetWaitStatisticsDTO
    {
        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 频道编号
        /// </summary>
        public string ChannelID { get; set; }

        public int UserLevel { get; set; }
    }
}
