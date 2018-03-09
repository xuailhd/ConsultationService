using System.ComponentModel;

namespace HealthCloud.Consultation.Enums
{
    [Description("WebAPI返回状态定义")]
    public enum EnumApiStatus
    {
        #region 默认业务状态 0~1
        [Description("操作成功")]
        BizOK = 0,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        BizError = 1,
        #endregion

        #region 系统接口状态 2~99
        /// <summary>
        /// 接口参数签名错误
        /// </summary>     
        [Description("接口签名参数错误")]
        ApiParamSignError = 2,
        /// <summary>
        /// 非法请求
        /// </summary>        
        [Description("接口用户令牌错误")]
        ApiParamTokenError = 3,
        /// <summary>
        /// 接口参数数据验证失败
        /// </summary>
        [Description("接口数据验证失败")]
        ApiParamModelValidateError = 4,
        /// <summary>
        /// 接口参数应用签名过期
        /// </summary>
        [Description("接口应用令牌过期")]
        ApiParamAppTokenExpire = 5,
        /// <summary>
        /// 接口时间戳参数错误
        /// </summary>
        [Description("接口时间戳参数错误")]
        ApiParamTimestampError = 9,
        /// <summary>
        /// 重复请求
        /// </summary>
        [Description("接口随机参数错误（重复请求)")]
        ApiRepeatedAccess = 8,
        /// <summary>
        /// 用户未登录
        /// </summary>
        [Description("用户未登录")]
        ApiUserNotLogin = 6,
        /// <summary>
        /// 用户无权限访问
        /// </summary>
        [Description("用户无权限访问")]
        ApiUserUnauthorized = 7,
        /// <summary>
        /// 操作成功
        /// </summary>      
        #endregion

        #region 子系统：远程会诊 200~299

        /// <summary>
        /// 患者不存在
        /// </summary>
        [Description("患者不存在")]
        PatientNotExists = 200,

        /// <summary>
        /// 还有未完成的会诊单
        /// </summary>
        [Description("还有未完成的会诊单")]
        HaveConsulNotFinised = 201,

        /// <summary>
        /// 只能有一个主诊医生
        /// </summary>
        [Description("只能有一个主诊医生")]
        OnlyAttendingDoctorOne = 202,

        /// <summary>
        /// 会诊医生不能大于6个
        /// </summary>
        [Description("会诊医生不能大于6个")]
        DoctorCountGTSix = 203,

        /// <summary>
        /// 会诊目的和要求不能为空
        /// </summary>
        [Description("会诊目的和要求不能为空")]
        PurposeNotEmpty = 204,

        /// <summary>
        /// 患者主诉，病情描述，初步诊断不能为空
        /// </summary>
        [Description("患者主诉，病情描述，初步诊断不能为空")]
        SubjectNotEmpty = 205,

        /// <summary>
        /// 还未分配主诊医生
        /// </summary>
        [Description("还未分配主诊医生")]
        NoAssignedDoctor = 206,

        /// <summary>
        /// 还未分配主诊医生
        /// </summary>
        [Description("还未分配会诊专家")]
        NoAssignedSpecialty = 207,

        /// <summary>
        /// 会诊单不存在
        /// </summary>
        [Description("会诊单不存在")]
        ConsultationNotExists = 208,

        /// <summary>
        /// 该状态不能修改
        /// </summary>
        [Description("该状态不能修改")]
        CurrStatusNotModify = 209,

        /// <summary>
        /// 已取消，已付款，已完成的订单不能再修改
        /// </summary>
        [Description("已取消，已付款，已完成的订单不能再修改")]
        CurrOrderStatusNotModify = 210,

        /// <summary>
        /// 手机号已存在
        /// </summary>
        [Description("手机号已存在")]
        MobileIsExists = 211,

        /// <summary>
        /// 手机号和姓名不能为空
        /// </summary>
        [Description("手机号和姓名不能为空")]
        MobileAndNameNotEmpty = 212,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        OfflinePayed = 213,

        /// <summary>
        /// 订单不存在
        /// </summary>
        [Description("支付订单还未创建")]
        OrderNoExists = 214,

        /// <summary>
        /// 未上传支付附件
        /// </summary>
        [Description("请上传付款附件")]
        NoPayedFile = 215,

        /// <summary>
        /// 支付金额与订单金额不一致
        /// </summary>
        [Description("支付金额与订单金额不一致")]
        AmountInconsistent = 216,

        /// <summary>
        /// 不能更改会诊状态
        /// </summary>
        [Description("不能更改会诊状态")]
        ConsultationNotChangeProgress = 217,

        /// <summary>
        /// 订单已取消，不能支付
        /// </summary>
        [Description("订单已取消不能支付")]
        InvalidNoPay = 218,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款，等待系统处理")]
        OfflineRefunded = 219,

        /// <summary>
        /// 不能线下退款
        /// </summary>
        [Description("该订单不能线下退款")]
        CannotOfflineRefunded = 220,

        /// <summary>
        /// 操作过快，请输后再试
        /// </summary>
        [Description("操作过快，请输后再试")]
        OperationTooFast = 221,

        #endregion

        #region 子系统：诊室 700-799

        /// <summary>
        /// 连接未就绪,请稍后重试
        /// </summary>
        [Description("连接未就绪,请稍后重试")]
        BizChannelNotReady = 700,

        /// <summary>
        /// 拒绝进入诊室，请在预约时间内或提前30分钟进入诊室
        /// </summary>
        [Description("拒绝进入诊室，请在预约时间内或提前30分钟进入诊室")]
        BizChannelRejectConnectIfNoReservationTime = 701,

        /// <summary>
        /// 拒绝设置状态，当前状态不是预期状态
        /// </summary>
        [Description("拒绝设置状态，当前状态不是预期状态")]
        BizChannelRejectSetStateIfNotExpectedState = 702,

        /// <summary>
        /// 拒绝设置状态，设置状态超时
        /// </summary>
        [Description("设置状态超时")]
        BizChanneSetStateIfTimeout = 703,

        /// <summary>
        /// 拒绝设置状态，当前诊室已失效
        /// </summary>
        [Description("拒绝设置状态，当前诊室已失效")]
        BizChannelSetStateIfClose = 704,


        /// <summary>
        /// 拒绝进入诊室，医生当前正在休诊
        /// </summary>
        [Description("拒绝进入诊室，医生当前正在休诊")]
        BizChannelRejectConnectIfDiagnoseOff = 705,
        #endregion

        #region 子系统：医生任务 1100-1199

        /// <summary>
        /// 医生任务池是空的
        /// </summary>
        [Description("没有需要任务需要领取")]
        BizDoctorTaskPoolEmpty = 1100,

        /// <summary>
        /// 医生任务池是空的
        /// </summary>
        [Description("服务器忙，请稍后重试")]
        BizDoctorTaskAlreadyTaskUnhandledFinish = 1101,

        #endregion

        #region 短信超频1401
        [Description("短信超频")]
        BizSMSOverclock = 1401,

        #endregion

        //更多的状态自己可以添加（业务状态需大于100，例如10100，10200这样可保存一定的扩展性）
    }
}