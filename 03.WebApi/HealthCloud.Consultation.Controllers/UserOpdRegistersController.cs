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
    /// 音视频预约控制器
    /// </summary>
    public class UserOPDRegistersController : ApiBaseController
    {
        private readonly UserOPDRegisterService userOPDRegisterService;

        /// <summary>
        /// 音视频预约控制器
        /// </summary>
        public UserOPDRegistersController()
        {
            userOPDRegisterService = new UserOPDRegisterService();
        }

        /// <summary>
        /// 新增看诊预约
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Insert([FromBody]RequestUserOPDRegisterSubmitDTO request)
        {
            var ret = userOPDRegisterService.Submit(request, true);

            //成功
            if (ret.ActionStatus == "Success")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //不支持
            else if (ret.ActionStatus == "UnSupport")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //预约重复
            else if (ret.ActionStatus == "Repeat")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            else if (ret.ActionStatus == "DiagnoseOff")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //预约失败
            else if (ret.ActionStatus == "Fail")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizError);
            }
            else
            {
                return new ApiResult() { Data = ret, Status = EnumApiStatus.BizError, Msg = ret.ErrorInfo };
            }
        }

        /// <summary>
        /// 查询是否已预约看诊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult TodaySubmited([FromBody]RequestCheckTodaySubmitedDTO request)
        {
            ResponseRepeatReturnDTO existsOrder;
            return userOPDRegisterService.ExistsWithSubmitRequest(request, out existsOrder).ToApiResultForBoolean();
        }

        /// <summary>
        /// 获取预约详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetEntity([FromBody]RequestGetEntityDTO request)
        {
            return userOPDRegisterService.Single(request.OPDRegisterID).ToApiResultForObject();
        }

        /// <summary>
        /// 获取服务详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetServiceDetail([FromBody]RequestGetEntityDTO request)
        {
            return userOPDRegisterService.GetServiceDetail(request.OPDRegisterID).ToApiResultForObject();
        }

        /// <summary>
        /// 获取预约记录列表
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetEntitys([FromBody]RequestQueryOPDRegisterDTO request)
        {
            return userOPDRegisterService.GetPageList(request).ToApiResultForList();
        }


        /// <summary>
        /// 获取医生的语音/视频看诊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetDoctorAudVid([FromUri]RequestQueryOPDRegisterDTO request)
        {
            var result = userOPDRegisterService.GetDoctorAudVid(request.CurrentOperatorDoctorID, request.CurrentPage, request.PageSize);
            return result.ToApiResultForList();
        }

        /// <summary>
        /// 获取机构预约记录列表
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult GetOrgOPDRegister([FromBody]RequestQueryOPDRegisterDTO request)
        {
            return userOPDRegisterService.GetOrgOPDRegister(request).ToApiResultForList();
        }

        /// <summary>
        /// 删除看诊预约
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult DeleteEntity([FromBody]RequestGetEntityDTO request)
        {
            return userOPDRegisterService.Delete(request.OPDRegisterID).ToApiResultForBoolean();
        }


        /// <summary>
        /// 转诊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult TransferTreatment([FromBody]RequestUserTransferTreatmentDTO request)
        {
            //throw new NotImplementedException("未实现转诊");
            var ret = userOPDRegisterService.TransferTreatment(request.OpdRegisterID, request.ToOPDType);
            //成功
            if (ret.ActionStatus == "Success")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //预约重复
            else if (ret.ActionStatus == "Repeat")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //套餐剩余次数不足
            else if (ret.ActionStatus == "UsedUp")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //不支持
            else if (ret.ActionStatus == "UnSupport")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizOK);
            }
            //预约失败
            else if (ret.ActionStatus == "Fail")
            {
                return ret.ToApiResultForObject(EnumApiStatus.BizError);
            }
            else
            {
                return new ApiResult() { Data = ret, Status = EnumApiStatus.BizError, Msg = ret.ErrorInfo };
            }
        }
    }


}
