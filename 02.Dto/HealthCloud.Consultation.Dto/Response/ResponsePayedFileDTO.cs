using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponsePayedFileDTO : ResponseUserFileDTO
    {
        public string OutID { get; set; }
        public string CreateUserName { get; set; }
        public string CreateUserID { get; set; }
        public DateTimeOffset CreateTime { get; set; }
    }
}
