using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using HealthCloud.Common.Json;
using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Common.Log;

namespace HealthCloud.Consultation.WebApi.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {

        public bool WithoutDetailException { get; set; }

        public ApiExceptionFilterAttribute():this(false)
        {

        }

        public ApiExceptionFilterAttribute(bool WithoutDetailException)
        {
            this.WithoutDetailException = WithoutDetailException;
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //记录日志
            var ex = actionExecutedContext.Exception.GetDetailException();
            LogHelper.DefaultLogger.Error(ex.Message, ex);
      
            //不包括详细异常信息（生产环境下）
            if (WithoutDetailException)
            {
                var result = EnumApiStatus.BizError.ToApiResultForApiStatus(actionExecutedContext.Exception.GetDetailException().Message);

                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonHelper.ToJson(result, false, ""),
                    System.Text.Encoding.UTF8,
                    "application/json")
                };
            }
            else
            {

                var result = EnumApiStatus.BizError.ToApiResultForApiStatus(
                    actionExecutedContext.Exception.GetDetailException(),
                    actionExecutedContext.Exception.GetDetailException().Message);

                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonHelper.ToJson(result, false, ""),
                    System.Text.Encoding.UTF8,
                    "application/json")
                };
            }

            actionExecutedContext.Response.StatusCode = HttpStatusCode.OK;

            base.OnException(actionExecutedContext);
        }
    }
}