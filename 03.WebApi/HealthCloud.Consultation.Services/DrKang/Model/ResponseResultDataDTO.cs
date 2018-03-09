using HealthCloud.Consultation.Services.DrKang.Model;
using System.Collections.Generic;

namespace HealthCloud.Consultation.Services.DrKang.Model
{
    public class ResponseResultDataDTO
    {
        public string body
        { get; set; }

        public string type { get; set; }

        public List<string> answer { get; set; }

        /// <summary>
        /// 诊断报告编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 消息编号
        /// </summary>
        public string msgId { get; set; }
    }



}