using HealthCloud.Common.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Common.Log;

namespace HealthCloud.Consultation.EventHandlers.DoctorAcceptEvent
{
    public class DefaultHandler : IEventHandler<Dto.EventBus.DoctorAcceptEvent>
    {
        public bool Handle(Dto.EventBus.DoctorAcceptEvent evt)
        {
            try
            {
                if (evt == null || string.IsNullOrEmpty(evt.DoctorID))
                    return true;

                DoctorTaskService bll = new DoctorTaskService();

                if (evt.ServiceType == EnumDoctorServiceType.VidServiceType || evt.ServiceType == EnumDoctorServiceType.AudServiceType)
                {
                    if (!bll.AcceptVideoCompleted(evt))
                    {
                        return false;
                    }
                }
                else if (evt.ServiceType == EnumDoctorServiceType.PicServiceType)
                {
                    if (!bll.AcceptTextConsultCompletd(evt))
                    {
                        return false;
                    }
                }

                #region 更新监控指标（记录处理订单的医生）

                SysMonitorIndexService service = new SysMonitorIndexService();
                var values = new Dictionary<string, string>();
                values.Add("DoctorID", evt.DoctorID);
                values.Add("DoctorName", evt.DoctorName);
                if (!service.InsertAndUpdate(new RequestSysMonitorIndexUpdateDTO()
                {
                    Category = "UserConsult",
                    OutID = evt.ServiceID,
                    Values = values
                }))
                {
                    return false;
                }
                #endregion

                return true;

            }
            catch (Dto.Exceptions.TaskConcurrentTakeException ex)
            {
                return true;
            }
            catch (Exception E)
            {
                LogHelper.DefaultLogger.Error(E.Message,E);
            }

            return false;
        }
    }


}
