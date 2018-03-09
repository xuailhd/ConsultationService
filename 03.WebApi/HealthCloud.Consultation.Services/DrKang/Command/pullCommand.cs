using KMEHosp.Hystrix;
using HealthCloud.Consultation.Services.DrKang.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang.Command
{
    /// <summary>
    /// 获取问题
    /// </summary>
    class pullCommand : HystrixCommand<ResponseResultDataDTO>
    {

        private static ResponseResultDataDTO fallback = new ResponseResultDataDTO()
        {
            type = EnumResultType.exception.ToString(),
            body = "",
            id = "",
           
        };

        string sessionID;

        public pullCommand(string sessionID) : base(HystrixCommandSetter.WithGroupKey("KMBAT")
                .AndCommandKey("pull")
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
            this.sessionID = sessionID;
        }

        protected override ResponseResultDataDTO Run()
        {
            var response = Helper.WebApiClient.Post<ResponseResultDataDTO>($"{Configuration.Config.ElasticsearchUrl}/elasticsearch/DrKang/pull?sessionID={sessionID}", "");

            if (response.resultCode == 0)
            {
                return response.resultData;

            }

            return fallback;
        }

        /// <summary>
        /// 失败时默认处理
        /// </summary>
        /// <returns></returns>
        protected override ResponseResultDataDTO GetFallback()
        {
            return fallback;
        }
    }
}
