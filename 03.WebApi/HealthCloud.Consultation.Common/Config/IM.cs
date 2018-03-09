using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Common.Config
{
    /// <summary>
    /// 短信配置
    /// </summary>
    public class IM
    {
        public string accountType
        { get; set; }

        public string sdkAppID
        { get; set; }

        /// <summary>
        /// 管理员账号（服务端集成消息是需要）
        /// </summary>
        public string adminAccount { get; set; }
    }
}
