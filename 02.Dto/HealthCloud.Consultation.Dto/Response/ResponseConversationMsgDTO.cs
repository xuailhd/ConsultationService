using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// 会话内容
    /// </summary>
    public class ResponseConversationMsgDTO
    {
        /// <summary>
        /// 会话内容ID
        /// </summary>        
        public string ConversationMessageID { get; set; }

        /// <summary>
        /// 会话房间ID
        /// </summary>
        public int ConversationRoomID { get; set; }

        /// <summary>
        /// 业务ID(关联外部业务ID)
        /// </summary>
        public string ServiceID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTimeOffset MessageTime { get; set; }

        /// <summary>
        /// 内容状态(0-未读、1-已读)
        /// </summary>
        public int MessageState { get; set; }

        /// <summary>
        /// 回调消息序号
        /// </summary>
        public string MessageSeq { get; set; }

        /// <summary>
        /// 回调消息数组索引号
        /// </summary>
        public int MessageIndex { get; set; }

    }
}
