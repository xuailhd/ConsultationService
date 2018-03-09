using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Common.Config
{
    public class Sysconfig
    {
        /// <summary>
        /// 咨询默认分钟数
        /// </summary>
        public string ConsultDefMinute { set; get; }

        /// <summary>
        /// 视频默认分钟数
        /// </summary>
        public string VideoDefMinute { set; get; }

        /// <summary>
        /// 休诊默认时长（秒）
        /// </summary>
        public string DiagnoseOffDefDuration { set; get; }
    }
}
