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
    /// 导诊
    /// </summary>
    class setBaseMsgCommand : HystrixCommand<ResponseResultDataDTO>
    {

        private static ResponseResultDataDTO fallback = new ResponseResultDataDTO()
        {
            type = EnumResultType.exception.ToString(),
            body = "",
            id = ""
        };

        string name;
        string birthday;
        string des;
        string gender;
        string deviceID;

        public setBaseMsgCommand(string name, string birthday, string des, string gender, string deviceID)
            : base(HystrixCommandSetter.WithGroupKey("KMBAT")
                .AndCommandKey("setBaseMsg")
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

            this.name = name;
            this.birthday = birthday;
            this.des = des;
            this.gender = gender;
            this.deviceID = deviceID;
        }

        protected override ResponseResultDataDTO Run()
        {
            var param = $"name={name}&birthday={birthday}&desc={des}&gender={gender}&userDeviceId={deviceID}";

            var response = Helper.WebApiClient.Post<ResponseResultDataDTO>($"{Configuration.Config.ElasticsearchUrl}/elasticsearch/DrKang/setBaseMsg?{param}", "");

            if (response.resultCode == 0)
            {
                return response.resultData;
            }
            else
            {
                return fallback;
            }

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
