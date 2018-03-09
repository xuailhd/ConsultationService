using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelCreatedEvent
{
    /// <summary>
    /// 图文咨询房间被创建后，发送用户预约时提交的图片信息到IM中
    /// 
    /// 触发条件：房间创建完成
    /// 前置条件：房间已经创建
    /// 后置条件：发送IM消息
    /// </summary>
    public class IfVideoOrAudioConsultSendConsultContent : IEventHandler<Dto.EventBus.ChannelCreatedEvent>
    {
        UserOPDRegisterService service = new UserOPDRegisterService();

        public bool Handle(Dto.EventBus.ChannelCreatedEvent evt)
        {

            if (evt != null && (evt.ServiceType == EnumDoctorServiceType.AudServiceType || evt.ServiceType == EnumDoctorServiceType.VidServiceType))
            {
                var lockName = $"{nameof(IfVideoOrAudioConsultSendConsultContent)}:{evt.ChannelID}";
                var lockValue = Guid.NewGuid().ToString("N");
                if (lockName.Lock(lockValue,TimeSpan.FromSeconds(5)))
                {
                    try
                    {
                        var CacheKey_Derep = new StringCacheKey(StringCacheKeyType.SysDerep_ChannelConsultContentMsg, evt.ChannelID.ToString());

                        //订单续费去重复
                        if (!CacheKey_Derep.FromCache<bool>())
                        {
                            if (service.SendConsultContent(evt.ChannelID, evt.ServiceID))
                            {
                                true.ToCache(CacheKey_Derep, TimeSpan.FromMinutes(5));                              
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex);
                        return false;
                    }
                    finally
                    {
                        lockName.UnLock(lockValue);
                    }
                }
                else
                {
                    return false;
                }
            }
                       
            return true;
        }
    }


}
