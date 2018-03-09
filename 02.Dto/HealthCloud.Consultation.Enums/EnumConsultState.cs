using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace HealthCloud.Consultation.Enums
{
    /// <summary>
    /// 预约状态 (0-未筛选/未支付、1-未领取、2-已领取/未回复、4-已回复/就诊中、5-已完成、6-已取消)
    /// </summary>
    [Description("咨询类型")]
    public enum EnumOPDState
    {
        /// <summary>
        /// 未支付
        /// </summary>
        [Description("未支付")]
        NoPay = 0,
        /// <summary>
        /// 未领取
        /// </summary>
        [Description("未领取")]
        NoReceive = 1,
        /// <summary>
        /// 已领取/未回复
        /// </summary>
        [Description("未回复")]
        NoReply = 2,
        /// <summary>
        /// 已回复/就诊中
        /// </summary>
        [Description("已回复")]
        Replied = 4,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 5,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 6
    }
}
