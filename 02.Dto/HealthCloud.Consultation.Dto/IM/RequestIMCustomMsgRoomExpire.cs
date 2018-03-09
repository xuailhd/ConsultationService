using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.IM
{
    
    public class RequestCustomMsgRoomExpire: IRequestIMCustomMsg<RequestConversationRoomRenewUpgradeDTO>
    {
        public RequestConversationRoomRenewUpgradeDTO Data
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
                return EnumIMCustomMsgType.Room_Expire.GetEnumDescript();
            }
        }

    }


}
