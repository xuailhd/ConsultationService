using HealthCloud.Consultation.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseDoctorInfoDTO : ImageBaseDto
    {
        /// <summary>
        /// 医生ID
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 医生名称
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// 医院名称
        /// </summary>
        public string HospitalName { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 医生会诊价格
        /// </summary>
        public decimal ConsulServicePrice { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 会诊次数
        /// </summary>
        public int ConsultationCount { get; set; }

        /// <summary>
        /// 医生图像
        /// </summary>
        string _PhotoUrl;
        public string PhotoUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_PhotoUrl))
                {
                    return PaddingUrlPrefix("images/doctor/default.jpg");

                }
                else
                {
                    return PaddingUrlPrefix(_PhotoUrl);
                }
            }
            set
            {
                _PhotoUrl = value;
            }
        }

        
        public int identifier { get; set; }

        public bool IsAttending { get; set; }

        /// <summary>
        /// 会诊意见
        /// </summary>
        public string Opinion { get; set; }

        /// <summary>
        /// 会诊价格
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 治疗方案
        /// </summary>
        public string Perscription { get; set; }

        public DateTimeOffset? ModifyTime { get; set; }

        public string UserAccount { get; set; }
        /// <summary>
        /// 是否是当前登录用户本人
        /// </summary>
        public bool IsCurrentUser { get; set; }

    }
}
