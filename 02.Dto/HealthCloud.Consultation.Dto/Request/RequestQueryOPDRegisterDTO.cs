using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 搜索请求
    /// </summary>
    public class RequestQueryOPDRegisterDTO : Common.IPagerRequest, Common.IRequestKeywordQuery
    {
        /// <summary>
        /// 搜索请求
        /// </summary>
        public RequestQueryOPDRegisterDTO()
        {
            PageSize = int.MaxValue;
        }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTimeOffset? BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }


        ///查询关键字
        /// <summary>
        /// 搜索关键字
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Keyword { get; set; }

        /// <summary>
        /// 分页索引
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public string OrgnazitionID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 就诊人ID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// 就诊人身份证号
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        public EnumDoctorServiceType? OPDType { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public EnumOPDState? OPDState { get; set; }

        /// <summary>
        /// 是否药店看诊账号
        /// </summary>
        public bool isTreatAccount { get; set; }

        /// <summary>
        /// 是否过滤排班
        /// </summary>
        public bool? FilterRecipeAndSchedule { get; set; }

        /// <summary>
        /// 当前操作医生ID
        /// </summary>
        public string CurrentOperatorDoctorID { get; set; }
    }
}