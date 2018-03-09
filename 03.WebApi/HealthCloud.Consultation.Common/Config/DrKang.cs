using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Common.Config
{
    /// <summary>
    /// 康博士
    /// </summary>
    public class DrKang 
    {
        public DrKang()
        {

        }

        public string Url
        { get; set; }
        
        /// <summary>
        /// 推送服务地址
        /// </summary>
        public string JPushUrl { get; set; }

        /// <summary>
        /// 搜索服务地址
        /// </summary>
        public string ElasticsearchUrl { get; set; }

        /// <summary>
        /// 康博士状态（是否启用）
        /// </summary>
        public string drKangEnable { get; set; }

    }
}
