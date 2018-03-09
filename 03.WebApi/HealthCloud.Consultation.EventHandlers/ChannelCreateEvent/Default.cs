using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChanneCreateEvent
{
    /// <summary>
    /// 创建频道
    /// 
    /// 触发条件：订单支付成功
    /// 前置条件：订单已支付
    /// 后置条件：调用云通信接口创建频道、更新频道启用状态
    /// </summary>
    public class DefaultHandler : IEventHandler<Dto.EventBus.ChannelCreateEvent>
    {
        public bool Handle(Dto.EventBus.ChannelCreateEvent evt)
        {
            try
            {
                if (evt == null)
                    return true;

                if (string.IsNullOrEmpty(evt.ChannelID))
                    return true;
              
                if (evt.ServiceType == EnumDoctorServiceType.PicServiceType || evt.ServiceType == EnumDoctorServiceType.VidServiceType || evt.ServiceType == EnumDoctorServiceType.AudServiceType)
                {
                    return new UserOPDRegisterService().CreateIMRoom(evt.ServiceID);
                }
                else if (evt.ServiceType == EnumDoctorServiceType.Consultation)
                {
                    return new RemoteConsultationService().CreateIMRoom(evt.ServiceID);
                }
                else if (evt.ServiceType == EnumDoctorServiceType.Registration)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception E)
            {
                LogHelper.DefaultLogger.Error(E);
            }

            return false;
        }
    }


}
