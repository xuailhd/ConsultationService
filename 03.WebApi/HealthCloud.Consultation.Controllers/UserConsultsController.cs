using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HealthCloud.Consultation.Controllers
{
    /// <summary>
    /// 用户咨询控制器
    /// </summary>
    public class UserConsultsController : ApiBaseController
    {
        private readonly UserOPDRegisterService userOPDRegisterService;
        private readonly ConversationRoomService conversationRoomService;

        /// <summary>
        /// 用户咨询控制器
        /// </summary>
        public UserConsultsController()
        {
            userOPDRegisterService = new UserOPDRegisterService();
            conversationRoomService = new ConversationRoomService();
        }
        /// <summary>
        /// 我的咨询（用户）
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Consulted([FromBody]RequestUserConsultsQueryDTO request)
        {
            return userOPDRegisterService.GetConsultedPageList(request).ToApiResultForList();
        }


        /// <summary>
        /// 咨询我的记录（医生）
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult ConsultMe([FromBody]RequestUserConsultsQueryDTO request)
        {
            return userOPDRegisterService.GetConsultMePageList(request).ToApiResultForList();
        }


        /// <summary>
        /// 新增咨询
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Submit([FromBody]RequestUserOPDRegisterSubmitDTO request)
        {
            var result = userOPDRegisterService.Submit(request);

            //预约成功
            if (result.ActionStatus == "Success")
            {
                return result.ToApiResultForObject();
            }

            return new ApiResult() { Data = result, Status = EnumApiStatus.BizError, Msg = result.ErrorInfo };

        }

        /// <summary>
        /// 创建报告解读
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResult CreateInterpretationConsultingRoom([FromBody]RequestUserOPDRegisterSubmitDTO request)
        {
            request.ConsultContent = "报告解读";
            return userOPDRegisterService.CreateConsultingRoom(request);
        }

        /// <summary>
        /// 创建报告解读
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResult ChangeInterpretationConsultingRoom([FromBody]RequestInterpretationRoomChangeDTO request)
        {
            try
            {
                var obj = userOPDRegisterService.Single(request.OriginalOPDRegisterID);
                var consult = new RequestUserOPDRegisterSubmitDTO();
                consult.MemberID = request.MemberID;
                consult.ConsultContent = obj.ConsultContent;
                consult.UserID = obj.UserID;
                consult.OrgnazitionID = obj.OrgnazitionID;
                if (obj.AttachFiles != null)
                {
                    obj.AttachFiles.ForEach(i => {
                        consult.Files.Add(new RequestUserFileDTO {
                            FileUrl = i.FileUrl.Replace(ImageBaseDto.UrlPrefix + "/", ""), Remark = i.Remark });
                    });
                }
                userOPDRegisterService.Cancel(request.OriginalOPDRegisterID);
                return userOPDRegisterService.CreateConsultingRoom(consult, null);
            }
            catch (Exception ex)
            {
                return EnumApiStatus.BizOK.ToApiResultForApiStatus(ex.Message);
            }
        }


        /// <summary>
        /// 查询医生的咨询记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult ConsultMeRecord([FromBody]RequestUserConsultsQueryDTO request)
        {
            var result = userOPDRegisterService.ConsultMeRecord(request.CurrentOperatorDoctorID, request.SelectType, request.CurrentPage, request.PageSize);
            return result.ToApiResultForList();
        }


        /// <summary>
        /// 查询是否已预约
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult TodayConsulted([FromBody]RequestCheckTodaySubmitedDTO request)
        {
            var existsOrder = new ResponseRepeatReturnDTO();
            var result = userOPDRegisterService.ExistsWithSubmitRequest(request, out existsOrder);
            return result.ToApiResultForBoolean();
        }

        /// <summary>
        /// 删除图文
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult DeleteEntity([FromBody]RequestGetEntityDTO request)
        {
            return userOPDRegisterService.Delete(request.OPDRegisterID).ToApiResultForBoolean();
        }
    }
}