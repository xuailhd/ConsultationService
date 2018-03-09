using KMEHosp.Hystrix;
using HealthCloud.Consultation.Services.DrKang.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCloud.Common.Json;
using HealthCloud.Common.Utility;

namespace HealthCloud.Consultation.Services.DrKang.Command
{
    /// <summary>
    /// 消息推送
    /// </summary>
    class msgPushCommand : HystrixCommand<bool>
    {

        private static bool fallback = false;

        string Content;
        string FromAccountId;
        string ToAlias;
        string OrderID;

        public msgPushCommand(string OrderID, string FromAccountId,string ToAlias, string Content) : base(HystrixCommandSetter.WithGroupKey("KMBAT")
                .AndCommandKey("msgPush")
                .AndCommandPropertiesDefaults(
                    new HystrixCommandPropertiesSetter()

                    //使用线程池隔离模式
                    .WithExecutionIsolationStrategy(ExecutionIsolationStrategy.Thread)
                    //执行超时则打断
                    .WithExecutionIsolationThreadInterruptOnTimeout(true)
                    //执行超时时间（100毫秒）
                    .WithExecutionIsolationThreadTimeoutInMilliseconds(5000)

                    //当在配置时间窗口内达到此数量的失败后，进行断开。默认20个）
                    .WithCircuitBreakerRequestVolumeThreshold(10)
                    //出错百分比阈值，当达到此阈值后，开始短路。默认50%
                    .WithCircuitBreakerErrorThresholdPercentage(50)
                    //多久以后开始尝试是否恢复，默认5s）
                    .WithCircuitBreakerSleepWindow(TimeSpan.FromSeconds(5))
            ))
        {

            this.Content = Content;
            this.FromAccountId = FromAccountId;
            this.ToAlias = ToAlias;
            this.OrderID = OrderID;
        }

        protected override bool Run()
        {

            string extras = "[{\"StrKey\":\"OrderId\",\"StrValue\":\"" + OrderID + "\"}]";
            var data = new
            {
                Title = "",
                Content = Content,
                FromAccountID = FromAccountId,
                Alias = ToAlias,
                ExtrasIOS = extras,
                ExtrasAndroid = extras
            };

            var ds = JsonHelper.ToJson(data);

            var apiUrl = Configuration.Config.JPushUrl.TrimEnd('/') + "/MessagePush/SendToAliasNetHospital";
            var model = WebAPIHelper.HttpPost(apiUrl, ds);

            return true;

        }

        /// <summary>
        /// 失败时默认处理
        /// </summary>
        /// <returns></returns>
        protected override bool GetFallback()
        {
            return fallback;
        }
    }
}
