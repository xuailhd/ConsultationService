﻿using System.ComponentModel;

namespace HealthCloud.Consultation.Enums
{

    /// <summary>
    /// 消费类型  0-付费 1-免费 2-义诊 3-套餐 5-家庭医生 6-机构折扣
    /// </summary>
    [Description("消费类型")]
    public enum EnumCostType
    {
        /// <summary>
        /// 付费
        /// </summary>
        [Description("付费")]
        Pay = 0,
        /// <summary>
        /// 免费
        /// </summary>
        [Description("免费")]
        Free =1,
        /// <summary>
        /// 义诊
        /// </summary>
        [Description("义诊")]
        FreeClinic = 2,
        /// <summary>
        /// 套餐
        /// </summary>
        [Description("套餐")]
        MemberPackage = 3,
        /// <summary>
        /// 家庭医生
        /// </summary>
        [Description("家庭医生")]
        FamilyDoctor = 5,

        /// <summary>
        /// 机构折扣
        /// </summary>
        [Description("机构折扣")]
        OrgDiscount = 6

    }
}
