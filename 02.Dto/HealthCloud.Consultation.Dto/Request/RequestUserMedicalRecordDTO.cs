using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthCloud.Consultation.Dto.Request
{

    /// <summary>
    /// 用户病历
    /// </summary>
    public class RequestUserMedicalRecordDTO
    {
        public RequestUserMedicalRecordDTO()
        {
            this.Sympton = "";
            this.PastMedicalHistory = "";
            this.PresentHistoryIllness = "";
            this.AllergicHistory = "";

        }

        public string UserMedicalRecordID
        {
            get;
            set;
        }

        /// <summary>
        /// 预约登记ID
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>    
        public string UserID { get; set; }



        public string MemberID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>     
        public string DoctorID { get; set; }


        /// <summary>
        /// 症状
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Sympton { get; set; }

        /// <summary>
        /// 既往病史
        /// </summary>

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PastMedicalHistory { get; set; }


        /// <summary>
        /// 现病史
        /// </summary>

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PresentHistoryIllness { get; set; }


        /// <summary>
        /// 初步诊断
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PreliminaryDiagnosis { get; set; }

        /// <summary>
        /// 患者主诉
        /// </summary>
        public string ConsultationSubject { get; set; }

        /// <summary>
        /// 病情描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 手术史
        /// </summary>
        public string PastOperatedHistory { get; set; }

        /// <summary>
        /// 家族史
        /// </summary>
        public string FamilyMedicalHistory { get; set; }

        /// <summary>
        /// 过敏史
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllergicHistory { get; set; }

        /// <summary>
        /// 医嘱
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Advised { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>      
        public string OrgnazitionID { get; set; }

        public string CurrentOperatorUserID { get; set; }

        public string OrgID { get; set; }
    }
}
