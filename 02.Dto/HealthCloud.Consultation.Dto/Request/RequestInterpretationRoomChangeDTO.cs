using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Request
{
    public class RequestInterpretationRoomChangeDTO
    {
        [Required]
        public string MemberID { get; set; }
        [Required]
        public string OriginalDoctorID { get; set; }
        [Required]
        public string OriginalOPDRegisterID { get; set; }
    }
}
