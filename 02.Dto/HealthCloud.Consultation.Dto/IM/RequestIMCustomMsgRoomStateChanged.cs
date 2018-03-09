using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.IM
{
    public class RequestCustomMsgRoomStateChanged:IRequestIMCustomMsg<RequestConversationRoomStatusDTO>
    {
        public RequestConversationRoomStatusDTO Data
        {
            get; set;
        }

        public string Desc
        {
            get; set;
        }

        public string Ext
        {
            get
            {
                return EnumIMCustomMsgType.Room_StateChanged.GetEnumDescript();
            }
        }
    }
}
