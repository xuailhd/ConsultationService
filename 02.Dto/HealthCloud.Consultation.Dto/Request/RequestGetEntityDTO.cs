using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 获取预约详细
    /// </summary>
    public class RequestGetEntityDTO
    {
        /// <summary>
        /// 预约详情ID
        /// </summary>
        [Required]
        public string OPDRegisterID { get; set; }
    }
}
