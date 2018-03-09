using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.IM;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Services.DrKang;
using HealthCloud.Consultation.Services.QQCloudy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelCreatedEvent
{
    /// <summary>
    ///呼叫康博士回答
    /// </summary>
    public class IfTextConsultCallDrKangAnswer : IEventHandler<Dto.EventBus.ChannelCreatedEvent>
    {
        DrKangService drKangService = new DrKangService();
        SysMonitorIndexService moniorIndexService = new SysMonitorIndexService();
        ConversationRoomService roomService = new ConversationRoomService();
        UserOPDRegisterService userOPDRegisterService = new UserOPDRegisterService();
        IIMHepler imservice = new IMHelper();

        public bool Handle(Dto.EventBus.ChannelCreatedEvent evt)
        {
            if (evt == null)
                return true;

            if (evt.ServiceType == EnumDoctorServiceType.PicServiceType)
            {
                var LockName = $"{typeof(IfTextConsultCallDrKangAnswer)}:{evt.ChannelID}";
                var lockValue = Guid.NewGuid().ToString("N");

                //获取分布式锁，获取锁失败时进行锁等待（锁超时时间2秒）
                if (LockName.Lock(lockValue,TimeSpan.FromSeconds(5)))
                {
                    try
                    {
                        var room = roomService.GetChannelInfo(evt.ChannelID);

                        #region 频道不可用则返回重试
                        if (!room.Enable)
                        {
                            return false;
                        }
                        #endregion

                        #region 发送用户的内容到聊天窗口
                        //避免重复，去重复
                        var CacheKey_Derep = new StringCacheKey(StringCacheKeyType.SysDerep_ChannelConsultContentMsg, evt.ChannelID.ToString());

                        if (!CacheKey_Derep.FromCache<bool>())
                        {
                            if (!userOPDRegisterService.SendConsultContent(evt.ChannelID, evt.ServiceID))
                            {
                                return false;
                            }
                            else
                            {
                                true.ToCache(CacheKey_Derep, TimeSpan.FromMinutes(5));
                            }
                        }
                        #endregion

                        #region 康博士处理

                        //已经启用了康博士
                        if (!string.IsNullOrEmpty(HealthCloud.Consultation.Services.DrKang.Configuration.Config.drKangEnable) &&
                                         (HealthCloud.Consultation.Services.DrKang.Configuration.Config.drKangEnable == "1" ||
                                         HealthCloud.Consultation.Services.DrKang.Configuration.Config.drKangEnable.ToUpper() == bool.TrueString.ToUpper())
                                         )
                        {
                            try
                            {
                                #region 检查当前咨询中康博士回答情况，判断是否还需要继续使用康博士
                                var cacheKey_Channel_DrKangState = new StringCacheKey(StringCacheKeyType.Channel_DrKangState, evt.ChannelID.ToString());
                                var Channel_DrKangState = cacheKey_Channel_DrKangState.FromCache<string>();

                                switch (Channel_DrKangState)
                                {
                                    //问答结束，没有匹配的疾病            
                                    case "nullMatchDisease":
                                    //问答结束，已有明确诊断
                                    case "diagnosis":
                                    //无法响应回复
                                    case "nullMatchResponse":
                                    //禁用(医生已回复)
                                    case "disabled":
                                    //出现异常
                                    case "exception":
                                        return true;
                                }
                                #endregion

                                var robotIdentifier = 0;
                                var robotName = "医生助理";
                                var robotPhotoUrl = "";

                                //图文咨询记录
                                var consult = userOPDRegisterService.Single(evt.ServiceID);

                                #region 图文咨询记录不存在、有医生处理、医生已经回复、咨询已完成、咨询已取消都直接忽略
                                if (consult == null ||
                                    consult.OPDState == EnumOPDState.Replied ||
                                    consult.OPDState == EnumOPDState.Completed ||
                                    consult.OPDState == EnumOPDState.Canceled
                                    )
                                {
                                    //记录最后一次问答的状态
                                    "disabled".ToCache(cacheKey_Channel_DrKangState);
                                    return true;
                                }
                                #endregion

                                #region 康博士加入到频道中
                                robotIdentifier = 0;
                                //康博士加入群组
                                if (!imservice.AddGroupMember(room.ConversationRoomID, new List<int>() { robotIdentifier }))
                                {
                                    return false;
                                }

                                //新增群组成员
                                if (!roomService.InsertChannelMembers(room.ConversationRoomID, new List<RequestChannelMemberDTO>() {
                                 new RequestChannelMemberDTO() {
                                      Identifier=robotIdentifier,
                                      UserID="",
                                      UserMemberID="",
                                      UserType= EnumUserType.SysRobot,
                                      PhotoUrl = robotPhotoUrl,
                                      UserENName =robotName,
                                      UserCNName =robotName
                                 }
                                }))
                                {
                                    return false;
                                }
                                #endregion

                                if (consult != null)
                                {
                                    //咨询内容不为空
                                    if (!string.IsNullOrEmpty(consult.ConsultContent))
                                    {
                                        if (consult.Member != null)
                                        {
                                            #region 使用康博士，记录最后的处理状态，并返回康博士的回答
                                            var SayHello = "";
                                            var QuestionTopic = "";
                                            var QuestionAnswer = new List<string>();

                                            var ret = drKangService.setBaseMsg(
                                                consult.Member.MemberName,
                                                "",
                                                 consult.ConsultContent,
                                                 consult.Member.Gender == EnumUserGender.Male ? "男" : "女",
                                                 consult.OPDRegisterID);

                                            //没有与症状匹配的模板
                                            if (ret.type == "nullMatchTemplate")
                                            {
                                                //康博士无能为力，医生参与吧
                                                SayHello = "您好，我是医生的助理！为了更好的为您服务，需要了解您的病情";
                                                QuestionTopic = "请详细描述您的症状";
                                            }
                                            else if (ret.type == "nullMatchSymptom")
                                            {
                                                //康博士无能为力，医生参与吧
                                                SayHello = "您好，我是医生的助理！为了更好的为您服务，需要了解您的病情";
                                                QuestionTopic = "请详细描述您的症状";
                                            }
                                            //匹配到多个模板需要跟用户确认（）
                                            else if (ret.type == "confirmTemplate")
                                            {
                                                //返回提示内容,需要在跟患者确认。
                                                SayHello = "您好，我是医生的助理！为了更好的为您服务，需要了解您的病情";
                                                QuestionTopic = ret.body;
                                            }
                                            //问答阶段
                                            else if (ret.type == "acking")
                                            {
                                                SayHello = "您好，我是医生的助理！为了更好的为您服务，需要了解您的病情";
                                                QuestionTopic = ret.body;
                                                QuestionAnswer = ret.answer;
                                                //返回提示信息，正在问答阶段，医生这时候是否能够介入？
                                            }
                                            //问答结束，没有匹配的疾病
                                            else if (ret.type == "nullMatchDisease")
                                            {
                                                //没有明确的诊断，需要医生参与
                                                SayHello = "您好，我是医生的助理！您的情况我已转达给了医生，请耐心等待医生的回复。";
                                                QuestionTopic = "";
                                            }
                                            //问答结束，已有明确诊断
                                            else if (ret.type == "diagnosis")
                                            {
                                                SayHello = "您好，我是医生的助理！您的情况我已转达给了医生，请耐心等待医生的回复。";
                                                QuestionTopic = ret.body;
                                                //返回诊断给客户
                                            }
                                            //无法回答
                                            else if (ret.type == "nullMatchResponse")
                                            {
                                                //康博士无法回答的问题，需人工介入
                                                SayHello = "您好，我是医生的助理！您的情况我已转达给了医生，请耐心等待医生的回复。";
                                                QuestionTopic = "";
                                            }

                                            //记录最后一次问答的状态
                                            ret.type.ToCache(cacheKey_Channel_DrKangState);
                                            #endregion

                                            #region 更新监控指标
                                            var values = new Dictionary<string, string>();
                                            values.Add("DrKangState", ret.type);//康博士问诊状态
                                            if (!moniorIndexService.InsertAndUpdate(new RequestSysMonitorIndexUpdateDTO()
                                            {
                                                Category = "UserConsult",
                                                OutID = consult.OPDRegisterID,
                                                Values = values
                                            }))
                                            {
                                                return false;
                                            }
                                            #endregion

                                            #region 使用非医生的身份，回答给用户

                                            //避免重复，去重复
                                            var CacheKey_DerepCallDrKangAnswerMsg = new StringCacheKey(StringCacheKeyType.SysDerep_ChannelCallDrKangAnswerMsg, evt.ChannelID.ToString());

                                            if (!CacheKey_DerepCallDrKangAnswerMsg.FromCache<bool>())
                                            {
                                                using (MQChannel channle = new MQChannel())
                                                {
                                                    channle.BeginTransaction();

                                                    #region 发送欢迎语句
                                                    if (!string.IsNullOrEmpty(SayHello))
                                                    {
                                                        channle.Publish(new Dto.EventBus.ChannelSendGroupMsgEvent<string>()
                                                        {
                                                            ChannelID = evt.ChannelID,
                                                            FromAccount = robotIdentifier,
                                                            Msg = SayHello
                                                        }, 2);
                                                    }
                                                    #endregion

                                                    #region 发送提问

                                                    //发送问题
                                                    if (!string.IsNullOrEmpty(QuestionTopic))
                                                    {

                                                        if (QuestionAnswer.Count > 0)
                                                        {
                                                            //发送自定义消息，客户端需要解析。采用点选的方式选择问题
                                                            channle.Publish(new Dto.EventBus.ChannelSendGroupMsgEvent<RequestIMCustomMsgSurvey>()
                                                            {
                                                                ChannelID = evt.ChannelID,
                                                                FromAccount = robotIdentifier,
                                                                Msg = new RequestIMCustomMsgSurvey()
                                                                {
                                                                    Desc = QuestionTopic,
                                                                    Data = new RadioTopic()
                                                                    {
                                                                        Answer = QuestionAnswer
                                                                    }
                                                                }
                                                            }, 4);
                                                        }
                                                        else
                                                        {
                                                            //发送文字消息
                                                            channle.Publish(new Dto.EventBus.ChannelSendGroupMsgEvent<string>()
                                                            {
                                                                ChannelID = evt.ChannelID,
                                                                FromAccount = robotIdentifier,
                                                                Msg = QuestionTopic
                                                            }, 4);
                                                        }
                                                    }

                                                    #endregion

                                                    channle.Commit();

                                                    true.ToCache(CacheKey_DerepCallDrKangAnswerMsg, TimeSpan.FromMinutes(5));

                                                    return true;
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        //记录没有设置基本信息（用户第一次提问时重试）
                                        "notSetBaseMsg".ToCache(cacheKey_Channel_DrKangState);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                #region 更新监控指标
                                var values = new Dictionary<string, string>();
                                values.Add("DrKangState", "exception");//康博士问诊状态
                                if (!moniorIndexService.InsertAndUpdate(new RequestSysMonitorIndexUpdateDTO()
                                {
                                    Category = "UserConsult",
                                    OutID = room.ServiceID,
                                    Values = values
                                }))
                                {
                                    return false;
                                }
                                else
                                {

                                    return true;
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            return true;
                        }
                        #endregion
                    }
                    catch (Exception E)
                    {
                        HealthCloud.Common.Log.LogHelper.DefaultLogger.Error(E.Message, E);
                        return false;
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }
                }
                else
                {
                    return false;
                }
              
            }

            return true;
        }
    }


}
