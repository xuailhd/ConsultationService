using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.QQCloudy
{
    public static class Configuration
    {

        public static Common.Config.IM IMConfig { get; private set; }
        public static Common.Config.Agora MediaConfig { get; private set; }
        public static string adminAccount { get; private set; }
        public static void RegisterConfig(
            Common.Config.IM config,
            Common.Config.Agora mediaConfig)
        {
            IMConfig = config;
            MediaConfig = mediaConfig;

            if (IMConfig != null)
            {
                adminAccount = IMConfig.adminAccount;
            }

        }
    }
}
