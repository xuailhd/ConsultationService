using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    /// <summary>
    /// 图文执行查询DTO
    /// </summary>
    public class RequestUserConsultsQueryDTO
    {
        /// <summary>
        /// 图文执行查询DTO
        /// </summary>
        public RequestUserConsultsQueryDTO()
        {
            IncludeRemoved = 1;
            CurrentPage = 1;
            PageSize = 10;
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
        /// 用户UserID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 看诊MemberID
        /// </summary>
        public string MemberID { get; set; }

        /// <summary>
        /// 患者身份证号
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 支付类型选择 -1：免费，义诊；0：免费；1：义诊；2：付费，套餐，会员，家庭医生
        /// </summary>
        public int SelectType { get; set; }


        /// <summary>
        /// 排除已经移除
        /// </summary>
        public int IncludeRemoved { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public EnumOPDState? OPDState { get; set; }


        /// <summary>
        /// 是否已支付
        /// </summary>
        public bool IsPayed { get; set; }

        /// <summary>
        /// 问诊类型：0-图文咨询，1-报告解读
        /// </summary>
        public int? InquiryType { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumRecordOrderType? OrderType { get; set; }

        /// <summary>
        /// 当前操作医生ID
        /// </summary>
        public string CurrentOperatorDoctorID { get; set; }
    }
}
