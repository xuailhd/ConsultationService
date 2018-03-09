using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 用户图文咨询
    /// </summary>
    public class ResponseUserConsultDTO
    {
        /// <summary>
        /// 用户咨询ID
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 成员ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 咨询内容
        /// </summary>
        public string ConsultContent { get; set; }

        /// <summary>
        /// 咨询时间
        /// </summary>
        public DateTimeOffset ConsultTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTimeOffset? FinishTime { get; set; }

        public DateTimeOffset? AnswerTime { get; set; }

        public DateTimeOffset PayTime { get; set; }

        /// <summary>
        /// 咨询状态(0-未回复、1-已回复、2-已完成)
        /// </summary>
        public EnumOPDState OPDState { get; set; }

        /// <summary>
        /// 消费类型  0-付费 1-免费 2-义诊 3-套餐 5-家庭医生 6-机构折扣
        /// </summary>
        public EnumCostType CostType { get; set; }

        public List<ResponseUserFileDTO> UserFiles { get; set; }

        public ResponseUserMemberDTO UserMember { get; set; }

        public ResponseDoctorInfoDTO Doctor { get; set; }

        public ResponseConversationRoomDTO Room { get; set; }

        public List<ResponseConversationMsgDTO> Messages { get; set; }

        
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 就诊人姓名
        /// </summary>
        public string MemberName { get; set; }
        /// <summary>
        /// 问诊类型：0-图文咨询，1-报告解读
        /// </summary>
        public int InquiryType { get; set; }

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


        public string RecipeFileUrl { get; set; }

        /// <summary>
        /// 是否存在病历记录
        /// </summary>
        public bool IsExistMedicalRecord { get; set; }

        /// <summary>
        /// 分诊状态（0无，1待分诊，2分诊中，3已分诊）
        /// </summary>
        public EnumTriageStatus TriageStatus { get; set; }

    }


}
