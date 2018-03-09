using HealthCloud.Consultation.Dto.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseUserFileDTO : ImageBaseDto
    {
        public string FileID { get; set; }

        public string FileName { get; set; }

        public int FileType { get; set; }

        public string OutID { get; set; }

        public long FileSize { get; set; }

        string _FileUrl;
        /// <summary>
        /// 文件地址
        /// </summary>
        [Required]
        public string FileUrl
        {
            set
            {
                _FileUrl = value;
            }
            get
            {
                return PaddingUrlPrefix(_FileUrl);
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [Required]
        public string Remark { get; set; }
    }
}
