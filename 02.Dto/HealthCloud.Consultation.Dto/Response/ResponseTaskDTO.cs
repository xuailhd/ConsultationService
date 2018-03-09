using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 视频问诊记录
    /// </summary>
    public class ResponseTaskDTO
    {
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>       
        public string DoctorID { get; set; }

        public string MemberID { get; set; }

        public string UserID { get; set; }

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
        public string Address { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }

        public string MedicalCardNumber { get; set; }

        /// <summary>
        /// 医生名称
        /// </summary>       
        public string DoctorName { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public DateTimeOffset RegDate { get; set; }

        /// <summary>
        /// 排班日期
        /// </summary>
        public DateTimeOffset OPDDate { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTimeOffset? PayTime { get; set; }

        /// <summary>
        /// 病情描述
        /// </summary>
        public string ConsultContent { get; set; }

        /// <summary>
        /// 是否已下诊断
        /// </summary>
        public bool? IsDiagnosed { set; get; }

        /// <summary>
        /// 已签名处方数量
        /// </summary>
        public int? RecipeSignedCount { set; get; }

        public long TriageID { get; set; }


        /// <summary>
        /// 房间状态
        /// </summary>
        public EnumRoomState RoomState { get; set; }

        /// <summary>
        /// 计费状态
        /// </summary>
        public EnumRoomChargingState ChargingState { get; set; }

        public string ChannelID { get; set; }

        public int Priority { get; set; }

        /// <summary>
        /// 预约类型（0-挂号、1-图文、2-语音、3-视频）
        /// </summary>
        public EnumDoctorServiceType OPDType { get; set; }

        public List<ResponseConversationMsgDTO> Messages { get; set; }
    }
}
