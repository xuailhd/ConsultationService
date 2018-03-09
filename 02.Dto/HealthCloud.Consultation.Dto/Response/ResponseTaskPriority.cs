using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseTaskPriority
    {
        public string DoctorGroupID { get; set; }

        public string ServiceID { get; set; }

        public int Priority { get; set;}
    }
}
