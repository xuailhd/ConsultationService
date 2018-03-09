using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang.Model
{
    public enum EnumServiceType
    {
        //获取基础问题
        getBaseQuestion = 0,
        //智能随访（首次）
        followup_firstTime = 1,
        //智能随访
        followup = 3,
        /// <summary>
        /// 回访
        /// </summary>
        return_visit=3
    }
}
