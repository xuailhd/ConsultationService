using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Common.Utility;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelStateChangedEvent
{
    /// <summary>
    /// 开始视频录制
    /// 作者：郭明
    /// 日期：2017年10月13日
    /// </summary>
    public class IfEnterChannelStartRec : IEventHandler<Dto.EventBus.ChannelStateChangedEvent>
    {
        public bool Handle(Dto.EventBus.ChannelStateChangedEvent evt)
        {
            try
            {
                var apiConfig = SysConfigService.Get<VideoRecorder>();

                if (evt.State == EnumRoomState.InMedicalTreatment && !string.IsNullOrEmpty(evt.ChannelID))
                {
                    var response = WebAPIHelper.HttpGet($"{apiConfig.VideoRecorderApiUrl.TrimEnd('/')}/api/StartRecord?ChannelID={evt.ChannelID}", "");

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
