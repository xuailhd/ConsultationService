using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;


namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 分诊医生
    /// </summary>
    public class ResponseDoctorTriageDTO
    {
        /// <summary>
        /// 预约ID
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 分诊医生ID
        /// </summary>
        public string TriageDoctorID { get; set; }

        /// <summary>
        /// 分诊状态（0无，1待分诊，2分诊中，3已分诊）
        /// </summary>
        public EnumTriageStatus TriageStatus { get; set; }

        /// <summary>
        /// 是否要经过导诊系统
        /// </summary>
        public bool IsToGuidance { get; set; }

    }
}
