using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Common
{

    /// <summary>
    /// 关键字搜索请求
    /// </summary>
    public interface IRequestKeywordQuery 
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        string Keyword { get; set; }
    }
}
