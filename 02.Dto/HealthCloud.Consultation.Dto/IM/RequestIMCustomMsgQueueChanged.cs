using HealthCloud.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.IM
{
    public class RequestCustomMsgQueueChanged : IRequestIMCustomMsg<int>
    {
        public int Data
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
                return HealthCloud.Consultation.Enums.EnumIMCustomMsgType.QueueChanged.GetEnumDescript();
            }
        }
    }
}
