using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Common.Utility;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelTriageChangedEvent
{
    /// <summary>
    /// 频道分诊被改变时
    /// </summary>
    public class DefaultHandler : IEventHandler<Dto.EventBus.ChannelTriageChangeEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();


        public bool Handle(Dto.EventBus.ChannelTriageChangeEvent evt)
        {
            try
            {
                if (evt == null)
                {
                    return true;
                }

                #region 设置分诊编号 
                //var room = roomService.GetChannelInfo(evt.ChannelID);
                //room.TriageID = SeqIDHelper.GetSeqId();

                //修改就诊时间和开始就诊时间
                if (!roomService.UpdateTriageID(evt.ChannelID, SeqIDHelper.GetSeqId()))
                {
                    return false;
                }
                #endregion

                #region 发送候诊队列通知
                roomService.SendWaitingQueueChangeNotice(evt.DoctorID);
                #endregion
            }
            catch(Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex);
                return false;
            }

            return true;
        }
    }


}
