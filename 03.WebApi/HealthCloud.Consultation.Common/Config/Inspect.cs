using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Common.Config
{
    /// <summary>
    /// 影像文件上传配置
    /// </summary>
    public class Inspect
    {
        public string UploadFileUrl { set; get; }

        public string AppId { set; get; }

        public string AppKey { set; get; }
    }
}
