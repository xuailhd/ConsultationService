using HealthCloud.Common.Extensions;
using HealthCloud.Common.Json;
using HealthCloud.Common.Log;
using HealthCloud.Common.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang.Helper
{

    /// <summary>
    /// Web接口客户端代理
    /// 作者：郭明
    /// 日期：2017年3月25日
    /// </summary>
    internal class WebApiClient
    {
        /// <summary>
        ///  API返回单个实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ApiResult : ApiMessageResult
        {
            /// <summary>
            /// 数据
            /// </summary>
            public object resultData { get; set; }
        }

        /// <summary>
        /// 接收API的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ApiResult<T> : ApiResult
        {
            /// <summary>
            /// 数据
            /// </summary>
            public new T resultData { get; set; }

        }

        /// <summary>
        /// API返回消息基类
        /// </summary>
        public class ApiMessageResult
        {
            /// <summary>
            /// 接口业务状态
            /// </summary>
            public int resultCode { get; set; }

            /// <summary>
            /// 消息状态说明
            /// </summary>
            public string msg { get; set; }
        }

        public static ApiResult<T> Post<T>(string path, object param)
        {
            var requestTime = DateTime.Now;
            var requestParam = JsonSerialize(param);
            var response = "";
            try
            {
                response = WebAPIHelper.HttpPost(path, requestParam);
                return JsonDeserialize<ApiResult<T>>(response);
            }
            catch (Exception ex)
            {
                response = JsonSerialize(ex.GetDetailException());

                return new ApiResult<T>()
                {
                    resultCode = 1,
                    resultData = default(T),
                    msg = ex.Message
                };
            }
            finally
            {
                WriteTrackLog(path, "", requestParam, requestTime, response);
            }
        }

        public static ApiResult<T> Post<T>(string path, string param)
        {
            var requestTime = DateTime.Now;
            var response = "";
            try
            {
                response = WebAPIHelper.HttpPost(path, param);
                return JsonDeserialize<ApiResult<T>>(response);
            }
            catch(Exception ex)
            {
                response = JsonSerialize(ex.GetDetailException());

                return new ApiResult<T>() {
                    resultCode =1,
                    resultData = default(T),
                    msg = ex.Message };
            }
            finally
            {
                WriteTrackLog(path, "", param, requestTime, response);
            }
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        static T JsonDeserialize<T>(string jsonString)
        {
            return JsonHelper.FromJson<T>(jsonString);
        }
        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        static string JsonSerialize(object obj)
        {
            return JsonHelper.ToJson(obj);
        }

        static void WriteTrackLog(string requestUri, string comments, string RequestParamters, DateTime requestEnterTime, string Response)
        {
            //LogHelper.WriteTrackLog("TrackJKBATApiOperatorLog",
            // requestUri: requestUri,
            // comments: comments,
            // RequestParamters: RequestParamters,
            // requestEnterTime: requestEnterTime,
            // Response: Response);
        }
    }
}
