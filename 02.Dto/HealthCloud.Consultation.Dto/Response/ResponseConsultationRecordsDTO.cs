using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HealthCloud.Consultation.Enums;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseConsultationRecordsDTO
    {

        public string MemberName { get; set; }
        public EnumUserGender Gender { get; set; }
        public string Mobile { get; set; }
        public string IDNumber { get; set; }
        public string DoctorName { get; set; }
        public DateTimeOffset? StartTimeReal { get; set; }
        public DateTimeOffset? FinishTimeReal { get; set; }

    }
}
