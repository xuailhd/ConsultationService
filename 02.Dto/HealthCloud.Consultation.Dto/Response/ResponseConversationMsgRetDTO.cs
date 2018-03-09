using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseConversationMsgRetDTO
    {
        public List<string> MsgBody
        { get; set; }

        public string MsgSeq { get; set; }

        public DateTimeOffset MsgTime { get; set; }

        public string FromAccount { get; set; }

        public string ToGroupId { get; set; }        
    }
}
