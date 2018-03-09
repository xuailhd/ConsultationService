using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Dto.Response
{

    /// <summary>
    /// 预约信息
    /// </summary>
    public class ResponseUserOPDRegisterDTO
    {
        /// <summary>
        /// 预约登记ID
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctorID { get; set; }


        /// <summary>
        /// 医生分组编号
        /// </summary>       
        public string DoctorGroupID { get; set; }

        [Required]
        /// <summary>
        /// 排班ID
        /// </summary>
        public string ScheduleID { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public DateTimeOffset RegDate { get; set; }

        /// <summary>
        /// 排班日期
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public DateTimeOffset OPDDate { get; set; }


        [Required]
        /// <summary>
        /// 预约类型（0-挂号、1-图文、2-语音、3-视频）
        /// </summary>
        public EnumDoctorServiceType OPDType { get; set; }    
        
        public EnumCostType CostType { get; set; }


        /// <summary>
        /// 预约金额
        /// </summary>
        public decimal Fee { get; set; }


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
        public string Address { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 机构编号
        /// </summary>
        public string OrgnazitionID { get; set; }


        public string OrgName { set; get; }

        public string RecipeFileID { get; set; }

        public ResponseUserMemberDTO Member { get; set; }

        public ResponseDoctorInfoDTO Doctor { get; set; }

        public ResponseConversationRoomDTO Room { get; set; }

        public string ConsultContent { get; set; }

        public ResponseUserMedicalRecordDTO UserMedicalRecord { get; set; }

        public ResponseDoctorTriageDTO DoctorTriage { get; set; }

        /// <summary>
        /// 是否存病历资料
        /// </summary>
        public bool IsExistMedicalRecord { get; set; }

        /// <summary>
        /// 处方文件路径
        /// </summary>
        public string RecipeFileUrl { get; set; }


        /// <summary>
        /// 用户上传的附件
        /// </summary>
        public List<ResponseUserFileDTO> AttachFiles { get; set; }

        /// <summary>
        /// 是否可删除
        /// </summary>
        public bool Deletable
        {
            get
            {
                return OPDState == EnumOPDState.Canceled || OPDState == EnumOPDState.Completed;
            }
        }

        /// <summary>
        /// 是否可取消
        /// </summary>
        public bool Cancelable
        {
            get
            {
                return OPDState == EnumOPDState.NoPay || OPDState == EnumOPDState.NoReceive;
            }
        }

        /// <summary>
        /// 预约状态
        /// </summary>
        [Required]
        public EnumOPDState OPDState { get; set; }

        /// <summary>
        /// 就诊开始时间
        /// </summary>    
        public string OPDBeginTime { get; set; }

        /// <summary>
        /// 就诊结束时间
        /// </summary>
 
        public string OPDEndTime { get; set; } 

        /// <summary>
        /// 问诊疾病
        /// </summary>
        [Column(TypeName = "nvarchar")]
        [MaxLength(128)]
        public string ConsultDisease { get; set; }

        /// <summary>
        /// 是否是离线处方，离线处方的处方要通过审方平台才能获取到
        /// </summary>
        public bool? OfflineRecipe { get; set; }

    }

}
