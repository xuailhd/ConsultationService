using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Services;

namespace HealthCloud.Consultation.Controllers
{
    /// <summary>
    /// 医生领单业务
    /// </summary>
    public class TaskController : ApiBaseController
    {
        private readonly DoctorTaskService doctorTaskService;

        /// <summary>
        /// 医生领单业务
        /// </summary>
        public TaskController()
        {
            doctorTaskService = new DoctorTaskService();
        }

        /// <summary>
        /// 安全检查
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult Index()
        {
            return "ok".ToApiResultForObject();
        }

        /// <summary>
        /// 过号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Triage([FromBody]RequestConversationRoomTriageDTO request)
        {
            using (MQChannel mqChannel = new MQChannel())
            {
                return mqChannel.Publish(new Dto.EventBus.ChannelTriageChangeEvent()
                {
                    ChannelID = request.ChannelID,
                    DoctorID = request.CurrentOperatorDoctorID,
                    TriageID = HealthCloud.Common.Utility.SeqIDHelper.GetSeqId().ToString()
                }).ToApiResultForBoolean();
            }
        }

        /// <summary>
        /// 领取问题
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Take([FromBody]RequestTaskDTO request)
        {
            if (request.ServiceType == EnumDoctorServiceType.AudServiceType || request.ServiceType == EnumDoctorServiceType.VidServiceType)
            {
                return doctorTaskService.AcceptVideo(request.CurrentOperatorDoctorID, request.GroupList);
            }
            else if (request.ServiceType == EnumDoctorServiceType.PicServiceType)
            {
                return doctorTaskService.AcceptTextConsult(request.CurrentOperatorDoctorID, request.GroupList);
            }
            else
            {
                return EnumApiStatus.BizError.ToApiResultForApiStatus();
            }
        }

        /// <summary>
        /// 图文、音视频问诊叫号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Call([FromBody]RequestTaskDTO request)
        {
            var task = doctorTaskService.GetTaskList(new RequestQueryTaskDTO
            {
                DoctorID = request.CurrentOperatorDoctorID,
                OPDType = new List<EnumDoctorServiceType> { EnumDoctorServiceType.VidServiceType, EnumDoctorServiceType.AudServiceType },
                RoomState = new List<EnumRoomState> { EnumRoomState.Waiting },
                ResponseFilters = new List<string> { "" }
            }).FirstOrDefault();

            return task.ToApiResultForObject();
        }

        /// <summary>
        /// 获取领取统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetStatistics([FromBody]RequestTaskDTO request)
        {
            return doctorTaskService.GetAcceptStatistics(request.CurrentOperatorDoctorID, request.GroupList).ToApiResultForObject();
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetTaskList(RequestQueryTaskDTO request)
        {
            return doctorTaskService.GetTaskList(request).ToApiResultForList();
        }

        /// <summary>
        /// 获取未处理任务统计（未回复图文咨询以及候诊中音视频问诊数量）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetUntreatedStatistics([FromBody]RequestTaskDTO request)
        {
            return doctorTaskService.GetUntreatedStatistics(request.CurrentOperatorDoctorID).ToApiResultForObject();
        }
    }
}