using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Common.Config
{
    /// <summary>
    /// 配置节点处理程序
    /// 作者：郭明
    /// 日期：2016年8月1日
    /// </summary>
    public interface IConfigSectionHandler
    {
        TConfigSection GetSection<TConfigSection>(string PropNameSurfix);

        bool SetSection<TConfigSection>(TConfigSection config, string Surfix = "");
    }
}
