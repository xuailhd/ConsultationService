using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestUploadFileDTO
    {
        public string UrlPrefix { get; set; }

        public string FileName { get; set; }

        public string MD5 { get; set; }

        public long FileSize { get; set; }

        public string FileSeq { get; set; }
        public string AccessKey { get; set; }

    }
}
