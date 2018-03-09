using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseConsultationInfoDTO
    {
        /// <summary>
        /// 会诊ID
        /// </summary>
        public string ConsultationID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

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
        /// 实际开始时间
        /// </summary>
        public DateTimeOffset? StartTimeReal { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTimeOffset? FinishTimeReal { get; set; }

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

        public EnumOPDState OPDState { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

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

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 预约ID
        /// </summary>
        public string OPDRegisterID { get; set; }

        public ResponseUserMemberDTO Member { get; set; }

        public List<ResponseDoctorInfoDTO> Doctors { get; set; }

        public ResponseUserMedicalRecordDTO MedicalRecord { get; set; }

        public List<ResponseUserFileDTO> Files { get; set; }

        public ResponseConversationRoomDTO Room { get; set; }

        public List<ResponseUserInspectResultDTO> InspectResult { get; set; }

        public List<ResponsePayedFileDTO> PayedFiles { get; set; }

        public List<ResponsePayedFileDTO> RefundFiles { get; set; }

    }
}
