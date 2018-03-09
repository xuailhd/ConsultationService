using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Enums;
using HealthCloud.Common.Utility;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;

namespace HealthCloud.Consultation.EventHandlers.ChannelStateChangedEvent
{
    /// <summary>
    /// 停止视频录制
    /// 作者：郭明
    /// 日期：2017年10月13日
    /// </summary>
    public class IfLeaveChannelStopRec : IEventHandler<Dto.EventBus.ChannelStateChangedEvent>
    {
        public bool Handle(Dto.EventBus.ChannelStateChangedEvent evt)
        {
            try
            {
                var apiConfig = SysConfigService.Get<VideoRecorder>();

                if (evt.State == EnumRoomState.AlreadyVisit && string.IsNullOrEmpty(evt.ChannelID))
                {
                    var response = WebAPIHelper.HttpGet($"{apiConfig.VideoRecorderApiUrl.TrimEnd('/')}/api/Convert?ChannelID={evt.ChannelID}", "");

                    var result = 0;

                    if (int.TryParse(response, out result))
                    {
                        if (result > 0)
                        {
                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex.Message, ex);
                return false;
            }

            return true;
        }
    }
}
