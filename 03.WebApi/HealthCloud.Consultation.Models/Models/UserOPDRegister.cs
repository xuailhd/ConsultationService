using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Models
{

    /// <summary>
    /// 预约编号
    /// </summary>
    public partial class UserOPDRegister : AuditableEntity
    {
        public UserOPDRegister()
        {
            this.PayTime = new DateTime(1900, 1, 1);
            this.RegDate = DateTime.Now;
            this.ConsultContent = "";
            this.DoctorID = "";
            this.DoctorGroupID = "";
            this.MemberID = "";
            this.MemberName = "";
            this.ScheduleID = "";
            this.IsUseTaskPool = false;
            this.Gender = EnumUserGender.Other;
            this.Marriage = EnumUserMaritalStatus.Other;
            this.IDNumber = "";
            this.IDType = EnumUserCardType.IDCard;
            this.Mobile = "";
            this.Birthday = "";
            this.OPDState = EnumOPDState.NoPay;
            OrderNo = "";
        }

        /// <summary>
        /// 预约登记ID
        /// </summary>
        [Key, Required]
        [Column(TypeName = "nvarchar")]
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string UserID { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string UserAccount { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>       
        [Column(TypeName = "nvarchar")]
        [Required]
        public string DoctorID { get; set; }

        /// <summary>
        /// 医生名称
        /// </summary>       
        [Column(TypeName = "nvarchar")]
        [Required]
        public string DoctorName { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(255)]
        public string DoctorPhotoUrl { get; set; }

        /// <summary>
        /// 医生分组编号
        /// </summary>       
        [Column(TypeName = "nvarchar")]
        [Required]
        public string DoctorGroupID { get; set; }

        /// <summary>
        /// 排班ID
        /// </summary>
        [Column(TypeName = "nvarchar")]
        public string ScheduleID { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTimeOffset PayTime { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        [Required]
        public DateTimeOffset RegDate { get; set; }

        /// <summary>
        /// 排班日期
        /// </summary>
        [Required]
        public DateTimeOffset OPDDate { get; set; }

        /// <summary>
        /// 就诊开始时间
        /// </summary>
        [Required]
        public string OPDBeginTime { get; set; }

        /// <summary>
        /// 就诊结束时间
        /// </summary>
        [Required]
        public string OPDEndTime { get; set; }

        /// <summary>
        /// 咨询内容
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(400)]
        public string ConsultContent { get; set; }

        /// <summary>
        /// 问诊疾病
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public string ConsultDisease { get; set; }

        /// <summary>
        /// 预约类型（0-挂号、1-图文、2-语音、3-视频）
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumDoctorServiceType OPDType { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(50)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 预约金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal")]
        public decimal Fee { get; set; }

        /// <summary>
        /// 续费金额
        /// </summary>
        public decimal RenewFee { get; set; }


        [Required]
        public string MemberID { get; set; }

        /// <summary>
        /// 成员姓名
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string MemberName { get; set; }

        /// <summary>
        /// 性别（0-男、1-女、2-未知）
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumUserGender Gender { get; set; }

        /// <summary>
        /// 婚姻情况(0-未婚、1-已婚、2-未知)
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumUserMaritalStatus Marriage { get; set; }

        /// <summary>
        /// 患者年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        public string IDNumber { get; set; }

        /// <summary>
        /// 证件类型（0-身份证）
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumUserCardType IDType { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(20)]
        public string Mobile { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(256)]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar")]
        [MaxLength(255)]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar")]
        [MaxLength(10), MinLength(10)]
        public string Birthday { get; set; }

        [Column(TypeName = "nvarchar")]
        public string MedicalCardNumber { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [Required]
        public string OrgnazitionID { get; set; }

        /// <summary>
        /// 使用任务池
        /// </summary>
        [Required]
        public bool IsUseTaskPool { get; set; }

        /// <summary>
        /// 预约标识（0：默认看诊，1：私人医生看诊）
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTimeOffset? AcceptTime { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTimeOffset? AnswerTime { get; set; }

        /// <summary>
        /// 问诊类型：0-图文咨询，1-报告解读
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public int InquiryType { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTimeOffset? FinishTime { get; set; }

        /// <summary>
        /// 预约状态 (0-未筛选/未支付、1-未领取、2-已领取/未回复、4-已回复/就诊中、5-已完成、6-已取消)
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumOPDState OPDState { get; set; }

        /// <summary>
        /// 消费类型  0-付费 1-免费 2-义诊 3-套餐 5-家庭医生 6-机构折扣
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumCostType CostType { get; set; }
    }
}
