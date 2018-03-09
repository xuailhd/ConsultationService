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
    /// 获取诊断小结
    /// 作者：郭明
    /// 日期：2017年6月28日
    /// </summary>
    class getInterrogationRecordCommand : HystrixCommand<ResponseInterrogationRecordDTO>
    {

        private static ResponseInterrogationRecordDTO fallback = new ResponseInterrogationRecordDTO()
        {
            disease = "",
            trueSymptom = "",
            falseSymptom = "",
        };

        string id;

        public getInterrogationRecordCommand(string id) : base(HystrixCommandSetter.WithGroupKey("KMBAT")
                .AndCommandKey("getInterrogationRecord")
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

            this.id = id;
        }

        protected override ResponseInterrogationRecordDTO Run()
        {
            //{"resultCode":"0", "msg":"", "resultData":[{"id":"2005","userDeviceId":"77ac3b794c2849c494706e1cca2b68ca","userName":"郭明","age":"","gender":"女","template":"咳嗽","trueSymptom":"吸烟史,过敏体质,小于1周,咯血,胸痛,发热,低热（37.2°~38°）,小于1天,鼻塞,流涕,打喷嚏,咽喉肿痛","falseSymptom":null,"disease":"急性上呼吸道感染","createdOn":"2017.08.18","evaluate":null,"isEvaluate":0}]}
            var response = Helper.WebApiClient.Post<List<ResponseInterrogationRecordDTO>>($"{Configuration.Config.ElasticsearchUrl}/elasticsearch/DrKang/getInterrogationRecordList?userDeviceId={id}&page=0&pageSize=1", "");

            if (response.resultCode == 0)
            {
                if (response.resultData.Count > 0)
                    return response.resultData[0];

            }

            return fallback;
        }

        /// <summary>
        /// 失败时默认处理
        /// </summary>
        /// <returns></returns>
        protected override ResponseInterrogationRecordDTO GetFallback()
        {
            return fallback;
        }
    }
}
