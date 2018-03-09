using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseOPDRegisterAudVidDTO
    {
        /// <summary>
        /// 预约登记ID
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public DateTimeOffset RegDate { get; set; }

        /// <summary>
        /// 预约类型（0-挂号、1-图文、2-语音、3-视频）
        /// </summary>
        public EnumDoctorServiceType OPDType { get; set; }

        public DateTimeOffset OPDDate { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// 预约状态 (0-未筛选/未支付、1-未领取、2-已领取/未回复、4-已回复/就诊中、5-已完成、6-已取消)
        /// </summary>
        public EnumOPDState OPDState { get; set; }

        public string MemberID { get; set; }

        public string MemberName { get; set; }
        public string ConsultContent { get; set; }
        public string Birthday { get; set; }
        public EnumUserGender Gender { get; set; }
        public string GenderText {
            get
            {
                return this.Gender.GetEnumDescript();
            }
        }
        public string ChannelID { get; set; }

        int _Age;
        public int Age
        {
            get
            {
                if (_Age > 0)
                {
                    return _Age;
                }
                else
                {
                    return HealthCloud.Common.ToolHelper.GetAgeFromBirth(Birthday);

                }
            }
            set
            {
                _Age = value;
            }
        }
    }
}
