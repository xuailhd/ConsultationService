using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang.Model
{

    public class ResponseRequestDrKangServerDTO
    {
        /// <summary>
        /// 会话编号
        /// </summary>
        public string sessionId
        { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string body { get; set; }
    }
}
