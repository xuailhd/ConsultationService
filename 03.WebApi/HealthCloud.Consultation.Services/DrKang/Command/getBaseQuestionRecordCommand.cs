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
    /// 获取问题记录
    /// 作者：郭明
    /// 日期：2017年11月2日
    /// </summary>
    class getBaseQuestionRecordCommand : HystrixCommand<ResponseBaseQuestionRecordDataDTO>
    {

        private static ResponseBaseQuestionRecordDataDTO fallback = new ResponseBaseQuestionRecordDataDTO()
        {
            recordList = new List<ResponseBaseQuestionRecordDTO>()
        };

        string sessionID;

        public getBaseQuestionRecordCommand(string sessionID) : base(HystrixCommandSetter.WithGroupKey("KMBAT")
                .AndCommandKey("getBaseQuestionRecord")
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

        protected override ResponseBaseQuestionRecordDataDTO Run()
        {
            var response = Helper.WebApiClient.Post<ResponseBaseQuestionRecordDataDTO>($"{Configuration.Config.ElasticsearchUrl}/elasticsearch/DrKang/getBaseQuestionRecord?sessionId={sessionID}", "");

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
        protected override ResponseBaseQuestionRecordDataDTO GetFallback()
        {
            return fallback;
        }
    }
}
