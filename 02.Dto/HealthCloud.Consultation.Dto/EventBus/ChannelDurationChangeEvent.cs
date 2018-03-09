using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Common.EventBus;

namespace HealthCloud.Consultation.Dto.EventBus
{
    /// <summary>
    /// 频道时间变更时间
    /// </summary>
    public class ChannelDurationChangeEvent : BaseEvent, IEvent
    {
        public string ServiceID { get; set; }

        public int Duration { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set;}

        public string NewUpgradeOrderNo { get; set; }
    }
}
