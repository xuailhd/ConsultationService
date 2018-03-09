using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Response
{
    public class ResponseUserOPDRegisterSubmitDTO
    {
        public string OPDRegisterID
        { get; set; }

        public string ActionStatus { get; set; }

        public EnumOPDState OPDState { get; set; }

        public string ErrorInfo
        { get; set; }

        public string OrderNO { get; set; }

        public string MemberID { get; set; }

        public string ChannelID { get; set; }
    }
}
