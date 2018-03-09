using HealthCloud.Common.Cache;
using HealthCloud.Consultation.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Repositories
{
    /// <summary>
    /// 配置节点处理程序
    /// </summary>
    public class CacheConfigSectionHandler:IConfigSectionHandler
    {
        public TConfigSection GetSection<TConfigSection>(string PropNameSurfix="")
        {
            if (Manager.Instance != null)
                return Manager.Instance.StringGet<TConfigSection>(typeof(TConfigSection).FullName+PropNameSurfix);
            else
                return default(TConfigSection);
        }

        public bool SetSection<TConfigSection>(TConfigSection config,string Surfix="")
        {
            if (Manager.Instance != null)
            {
                Manager.Instance.StringSet(typeof(TConfigSection).FullName+Surfix, config);
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
