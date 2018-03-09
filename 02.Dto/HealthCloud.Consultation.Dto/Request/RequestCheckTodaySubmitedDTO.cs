using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.Request
{

    /// <summary>
    /// 预约检查
    /// </summary>
    public class RequestCheckTodaySubmitedDTO
    {
        /// <summary>
        /// 预约检查
        /// </summary>
        public RequestCheckTodaySubmitedDTO()
        {

        }

        /// <summary>
        /// 预约类型（0-挂号、1-图文、2-语音、3-视频）
        /// </summary>
        [Required]
        public EnumDoctorServiceType OPDType { get; set; }


        /// <summary>
        /// 就诊人成员ID
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required]
        public string MemberID { get; set; }


        /// <summary>
        /// 用户编号
        /// </summary>
        [Required]
        public string UserID { get; set; }

        
        /// <summary>
        /// 没有指定排版的时候需要指定医生编号
        /// </summary>
        [DisplayFormat(NullDisplayText = "")]
        public string DoctorID { get; set; }
    }
}
