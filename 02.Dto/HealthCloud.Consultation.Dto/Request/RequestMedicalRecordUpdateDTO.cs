using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestMedicalRecordUpdateDTO
    {
        /// <summary>
        /// 会诊ID
        /// </summary>
        public string ConsultationID { get; set; }

        public RequestUserMedicalRecordDTO MedicalRecord { get; set; }

        public List<RequestUserFileDTO> Files { get; set; }
        public string CurrentOperatorUserID { get; set; }

        public string OrgID { get; set; }
    }

}
