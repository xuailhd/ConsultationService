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
    /// 预约提交
    /// </summary>
    public class RequestUserOPDRegisterSubmitDTO
    {
        /// <summary>
        /// 预约提交
        /// </summary>
        public RequestUserOPDRegisterSubmitDTO()
        {

        }

        /// <summary>
        /// 预约ID
        /// </summary>
        [IgnoreInDoc]
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        [IgnoreInDoc]
        public string ChannelID { get; set; }

        /// <summary>
        /// 预约类型（0-挂号、1-图文、2-语音、3-视频）
        /// </summary>
        [Required]
        public EnumDoctorServiceType OPDType { get; set; }

        /// <summary>
        /// 消费类型  0-付费 1-免费 2-义诊 3-套餐 5-家庭医生 6-机构折扣
        /// </summary>
        [Required]
        public EnumCostType CostType { get; set; }

        /// <summary>
        /// 排班ID
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ScheduleID { get; set; }

        /// <summary>
        /// 就诊人成员ID
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required]
        public string MemberID { get; set; }

        /// <summary>
        /// 诊疗卡号
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MedicalCardID { get; set; }

        /// <summary>
        /// 诊疗卡医院编号
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MedicalCardHospID { get; set; }

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

        /// <summary>
        /// 机构ID
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required]
        public string OrgnazitionID
        { get; set; }

        /// <summary>
        /// 咨询内容
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(400)]
        public string ConsultContent { get; set; }

        /// <summary>
        /// 咨询疾病
        /// </summary>
        [MaxLength(30)]
        public string ConsultDisease { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public virtual List<RequestUserFileDTO> Files { get; set; }

        /// <summary>
        /// 就诊开始时间
        /// </summary>
        public string OPDBeginTime { get; set; }

        /// <summary>
        /// 就诊结束时间
        /// </summary>
        public string OPDEndTime { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public DateTimeOffset OPDDate { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [Required]
        public string UserID { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Required]
        public string UserAccount { get; set; }

        /// <summary>
        /// 用户等级级别
        /// </summary>
        [Required]
        public int UserLevel { get; set; }

        /// <summary>
        /// 问诊类型：0-图文咨询，1-报告解读
        /// </summary>
        public int InquiryType { get; set; } = 0;

        /// <summary>
        /// 没有指定排版的时候需要指定医生编号
        /// </summary>
        [DisplayFormat(NullDisplayText = "")]
        public string DoctorID { get; set; }


        /// <summary>
        /// 医生姓名
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// 医生头像
        /// </summary>
        public string DoctorPhotoUrl { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 续费金额
        /// </summary>
        [Required]
        public decimal RenewFee { get; set; }

        /// <summary>
        /// 预约标识（0：默认看诊，1：私人医生看诊）
        /// </summary>
        public int Flag { get; set; }


        /// <summary>
        /// 医生分组编号
        /// </summary>
        [DisplayFormat(NullDisplayText = "")]
        public string DoctorGroupID { get; set; }

        /// <summary>
        /// 过敏史
        /// </summary>
        public string AllergicHistory { get; set; }

        /// <summary>
        /// 服务金额
        /// </summary>
        [Required]
        public decimal ServicePrice { get; set; }

        /// <summary>
        /// 是否走订单池
        /// </summary>
        [IgnoreInDoc]
        public bool IsUseTaskPool { get; set; }


        /// <summary>
        /// 是否走导诊
        /// </summary>
        public bool IsToGuidance { get; set; }
    }
}
