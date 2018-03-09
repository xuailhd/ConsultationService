using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Enums
{
    /// <summary>
    /// 枚举出缓存Key的类型
    /// </summary>
    public enum StringCacheKeyType
    {
        #region 接口相关
        API_apptoken,
        API_usertoken,
        api_noncestr,
        #endregion


        /// <summary>
        /// 抢单
        /// </summary>
        Grab,

        Sys_TaskListResetTime,
        /// <summary>
        /// 系统消息通知扩展配置
        /// </summary>
        Sys_NoticeMessageExtrasConfig,

        SysDerep_ChannelConsultContentMsg,
        SysDerep_ChannelCallDrKangAnswerMsg,
        SysDerep_OrderNewupgrade,

        /// <summary>
        /// 频道信息
        /// </summary>
        Channel,
        /// <summary>
        /// 频道成员信息
        /// </summary>
        Channel_Member,
        Channel_DrKangState,
        Channel_DoctorAnswerState,

        MAP_GetChannelIDByServiceID,
        MAP_GetIMUidByUserID,

        Sys_UserLevelRules,

        Doctor_Configs,

        /// <summary>
        /// IM Key
        /// </summary>
        KEY_IM_PriaveKey,
        KEY_IM_PublicKey

    }
}
