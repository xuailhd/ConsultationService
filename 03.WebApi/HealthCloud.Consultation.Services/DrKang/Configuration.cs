using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang
{
    public static class Configuration
    {
        public static Common.Config.DrKang Config { get; private set; }

        public static void RegisterConfig(Common.Config.DrKang config)
        {
            Config = config;
        }
    }
}
