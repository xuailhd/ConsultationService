using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Common.EventBus;

namespace HealthCloud.Consultation.Dto.EventBus
{
    /// <summary>
    /// 房间过期时间
    /// </summary>
    public class ChannelExpireEvent : BaseEvent, IEvent
    {
        public string ServiceID { get; set; }
    }
}
