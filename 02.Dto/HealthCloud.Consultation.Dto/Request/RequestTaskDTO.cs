using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 领单
    /// </summary>
    public class RequestTaskDTO
    {
        /// <summary>
        /// 领单服务类型
        /// </summary>
        [Required]
        public EnumDoctorServiceType ServiceType { get; set; }

        /// <summary>
        /// 当前操作医生ID
        /// </summary>
        [Required]
        public string CurrentOperatorDoctorID { get; set; }

        /// <summary>
        /// 当前医生所属分组
        /// </summary>
        [Required]
        public List<string> GroupList { get; set; }
    }
}
