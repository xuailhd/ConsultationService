using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestConsultationInfoUpdateDTO
    {
        /// <summary>
        /// 会诊ID
        /// </summary>
        public string ConsultationID { get; set; }

        [Required]
        public string MemberID { get; set; }

        /// <summary>
        /// 成员姓名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 性别（0-男、1-女、2-未知）
        /// </summary>
        public EnumUserGender Gender { get; set; }

        /// <summary>
        /// 婚姻情况(0-未婚、1-已婚、2-未知)
        /// </summary>
        public EnumUserMaritalStatus Marriage { get; set; }

        /// <summary>
        /// 患者年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 证件类型（0-身份证）
        /// </summary>
        public EnumUserCardType IDType { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string PatientAddress { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }


        /// <summary>
        /// 会诊要求
        /// </summary>
        public string Requirement { get; set; }

        /// <summary>
        /// 会诊目的
        /// </summary>
        public string Purpose { get; set; }


        /// <summary>
        /// 会诊开始时间
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// 会诊完成时间
        /// </summary>
        public DateTimeOffset? FinishTime { get; set; }

        /// <summary>
        /// 会诊号
        /// </summary>
        public string ConsultationNo { get; set; }

        /// <summary>
        /// 押金
        /// </summary>
        public decimal Deposit { get; set; }

        /// <summary>
        ///  会诊费用
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 会诊状态
        /// </summary>
        public EnumConsultationProgress ConsultationProgress { get; set; }

        /// <summary>
        /// 会诊来源
        /// </summary>
        public EnumConsultationSource ConsultationSource { get; set; }

        /// <summary>
        /// 会诊地点
        /// </summary>
        public string Address { get; set; }

        public string OrgID { get; set; }

        /// <summary>
        /// 当前用户为主诊医生
        /// </summary>
        public bool CurrUserAttending { get; set; }

        public string CreateUserID { get; set; }
        public DateTimeOffset CreateTime { get; set; }

        public List<RequestConsultationDoctorDTO> Doctors { get; set; }

        public RequestUserMedicalRecordDTO MedicalRecord { get; set; }

        public List<RequestUserFileDTO> Files { get; set; }

        public List<RequestUserInspectResultDTO> InspectResult { get; set; }

        public string ConsultationDoctorID { get; set; }
        public string CurrentOperatorUserID { get; set; }

        /// <summary>
        /// 是否走导诊断
        /// </summary>
        public bool IsToGuidance { get; set; }

        /// <summary>
        /// 平台服务费
        /// </summary>
        public decimal PlatServicePrice { get; set; }
    }

}
