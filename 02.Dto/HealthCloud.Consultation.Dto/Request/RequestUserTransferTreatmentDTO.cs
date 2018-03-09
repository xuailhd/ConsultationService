using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestUserTransferTreatmentDTO
    {
        /// <summary>
        /// 预约编号
        /// </summary>
        [Required]
        public string OpdRegisterID { get; set; }

        /// <summary>
        /// 转诊的服务类型
        /// </summary>
        [Required]
        public EnumDoctorServiceType ToOPDType { get; set; }
    }
}
