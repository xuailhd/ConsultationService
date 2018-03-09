using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Repositories.EF;
using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Models;

namespace HealthCloud.Consultation.Services
{
    /// <summary>
    /// 会话消息业务逻辑
    /// </summary>
    public class ConversationMessageService 
    {
        public PagedList<ResponseConversationMsgRetDTO> GetMessages(string RoomChannelID, int CurrentPage, int PageSize)
        {
            using (var db = new DBEntities())
            {
                var query = from message in db.ConversationMessages.Where(a => a.IsDeleted == false)
                            where message.ConversationRoomID == RoomChannelID
                            group message by message.MessageSeq into gps
                            select new ResponseConversationMsgRetDTO()
                            {
                                MsgSeq = gps.Key,
                                FromAccount = gps.FirstOrDefault().UserID,
                                ToGroupId = gps.FirstOrDefault().ConversationRoomID.ToString(),
                                MsgTime = gps.FirstOrDefault().MessageTime,
                                MsgBody = gps.OrderBy(a => a.MessageIndex).Select(a => a.MessageContent).ToList(),
                            };


                query = query.OrderBy(a => new { a.MsgTime });


                return query.ToPagedList(CurrentPage, PageSize);

            }
        }


        public ResponseConversationMsgDTO Single(string conversationMessageID)
        {
            using (var db = new DBEntities())
            {
                var model = db.ConversationMessages.Where(t => t.ConversationMessageID == conversationMessageID).FirstOrDefault();
                return model.Map<ConversationMessage, ResponseConversationMsgDTO>();
            }
        }

        public bool Insert(List<ResponseConversationMsgDTO> msgs)
        {
            if(msgs == null)
            {
                return false;
            }

            using (var db = new DBEntities())
            {
                foreach(var msg in msgs)
                {
                    var dbmodel = msg.Map<ResponseConversationMsgDTO, ConversationMessage>();
                    dbmodel.ConversationMessageID = Guid.NewGuid().ToString("N");
                    db.ConversationMessages.Add(dbmodel);
                }
                return db.SaveChanges() > 0;
            }
        }
    }
}
