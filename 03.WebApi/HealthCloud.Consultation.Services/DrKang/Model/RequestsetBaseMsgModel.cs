using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang.Model
{
    internal class RequestsetBaseMsgModel
    {
        /// <summary>
        /// 性别
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string birthday { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string userDeviceId { get; set; }
    }
}
