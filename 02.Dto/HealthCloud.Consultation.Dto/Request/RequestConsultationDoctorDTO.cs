using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{

    public class RequestConsultationDoctorDTO
    {
        public bool IsAttending
        { get; set; }

        public string DoctorID
        { get; set; }

        public string DoctorName { get; set; }

        public string PhotoUrl { get; set; }

        public string DepartmentName { get; set; }

        public string HospitalName { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 服务价格
        /// </summary>
        public decimal ServicePrice { get; set; }
    }
}
