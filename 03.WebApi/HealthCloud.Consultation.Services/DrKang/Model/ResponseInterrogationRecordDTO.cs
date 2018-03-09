using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang.Model
{
    public class ResponseInterrogationRecordDTO
    {
        public string userName
        { get; set; }

        public string age
        { get; set; }

        public string gender
        { get; set; }

        public string template
        { get; set; }

        public string trueSymptom
        { get; set; }

        public string falseSymptom
        { get; set; }

        public string disease { get; set; }

        public string evaluate { get; set; }

        public int isEvaluate { get; set; }

    }
}
