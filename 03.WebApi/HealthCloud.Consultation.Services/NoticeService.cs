
using EntityFramework.Extensions;
using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Dto.Notice;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace HealthCloud.Consultation.Services
{
    public class NoticeService
    {
        /// <summary>
        /// 插入消息表并推送消息
        /// </summary>
        /// <param name="model">消息</param>
        /// <param name="ToUserList">接收消息的用户</param>
        /// <param name="pageArgs">扩展参数，该参数会一起推给客户端</param>
        /// <param name="silence">是否静默推送</param>
        /// <returns></returns>
        public bool SendNotice(
            RequestNoticeMessageDTO model,
            List<string> ToUserList = null,
            bool silence = false,
            string ServiceID = "",
            string FromUserID = "",
            EnumTargetUserCodeType ToUserListType = EnumTargetUserCodeType.UserID)
        {
            if (ToUserList == null)
            {
                ToUserList = new List<string>();
            }

            var extrasIOS = _GetJMessageExtras(EnumTerminalType.IOS, model, model.PageArgs);
            var extrasAndroid = _GetJMessageExtras(EnumTerminalType.Android, model, model.PageArgs);

            using (MQChannel mqChanne = new MQChannel())
            {
                return mqChanne.Publish(new Dto.EventBus.UserNoticeSendEvent()
                {
                    MessageID = model.MessageID,
                    Content = model.Content,
                    Title = model.Title,
                    Summary = model.Summary,
                    PageArgs = model.PageArgs,
                    ClientName = model.ClientName,
                    NoticeDate = model.NoticeDate,
                    NoticeSecondType = model.NoticeSecondType,
                    FromUserID = FromUserID,
                    ToUserList = ToUserList,
                    ToUserListType = ToUserListType,
                    extrasAndroid = extrasAndroid,
                    extrasIOS = extrasIOS,
                    serviceID = ServiceID,
                    silence = silence

                });
            }

        }

        /// <summary>
        /// 生成极光推送的扩展参数
        /// </summary>
        /// <param name="terminalType"></param>
        /// <param name="noticeMessage"></param>
        /// <param name="pageArgs"></param>
        /// <returns></returns>
        Dictionary<string, object> _GetJMessageExtras(EnumTerminalType terminalType, RequestNoticeMessageDTO noticeMessage, string pageArgs)
        {


            var dic = new Dictionary<string, object>();


            #region 处理兼容问题(老的APP应用通知类型长度)
            var NoticeType = ((int)noticeMessage.NoticeSecondType).ToString().PadLeft(7, '0');
            var MsgType = int.Parse(NoticeType.Substring(4, 3));

            //通知类型是20以下的都需要处理成兼容，后续的类型在20以上
            if (MsgType <= 20)
            {
                //老的类型需要兼容
                dic.Add("MsgType", MsgType);
            }
            else
            {
                dic.Add("MsgType", NoticeType);
            }
            #endregion
            dic.Add("NoticeType", NoticeType);
            dic.Add("MsgID", noticeMessage.MessageID);
            dic.Add("PageArgs", pageArgs);
            var model = GetMessageExtrasConfig(terminalType, noticeMessage.NoticeSecondType);
            if (model != null)
            {
                dic.Add("PageType", model.PageType);
                dic.Add("PageUrl", model.PageUrl);

            }
            return dic;
        }

        public RequestMessageExtrasConfigDTO GetMessageExtrasConfig(EnumTerminalType terminalType, EnumNoticeSecondType msgType)
        {
            var entity = GetAllExtrasConfig().Where(i => i.TerminalType == terminalType && i.MsgType == msgType).FirstOrDefault();
            return entity;
        }

        List<RequestMessageExtrasConfigDTO> GetAllExtrasConfig()
        {
            var cacheKey = new StringCacheKey(StringCacheKeyType.Sys_NoticeMessageExtrasConfig, "");
            ////从缓存中取
            var list = cacheKey.FromCache<List<RequestMessageExtrasConfigDTO>>();

            if (list == null)
            {

                using (DBEntities db = new DBEntities())
                {
                    list = db.MessageExtrasConfigs.Where(a => !a.IsDeleted).Select(a => new RequestMessageExtrasConfigDTO()
                    {

                        ExtrasConfigID = a.ExtrasConfigID,
                        MsgTitle = a.MsgTitle,
                        MsgType = a.MsgType,
                        PageArgs = "",
                        PageTarget = a.PageTarget,
                        PageType = a.PageType,
                        PageUrl = a.PageUrl,
                        TerminalType = a.TerminalType

                    }).ToList();
                }

                list.ToCache(cacheKey);
            }

            return list;
        }
    }
}
