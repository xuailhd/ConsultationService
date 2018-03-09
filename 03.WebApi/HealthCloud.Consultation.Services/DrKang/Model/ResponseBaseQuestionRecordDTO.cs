using HealthCloud.Consultation.Services.DrKang.Model;
using System.Collections.Generic;

namespace HealthCloud.Consultation.Services.DrKang.Model
{
    //{"resultCode":"0", "msg":"", "resultData":{"recordList":[{"record":[{"id":null,"question":"您感觉症状轻、中、重属于哪一种？","display":"症状程度","answer":"266。"}],"type":"症状程度"},{"record":[{"id":null,"question":"请问你哪里不舒服呢？有多久了？","display":"主要症状与持续时间","answer":"五五。"}],"type":"主要症状与持续时间"},{"record":[{"id":null,"question":"有没有看过医生？有没有做过检查？有没有服用过药物呢？","display":"病史","answer":"八八。"}],"type":"病史"},{"record":[{"id":null,"question":"您以前有得较大的疾病没？有没有什么慢性病？比如高血压、糖尿病、高血脂之类。","display":"既往史","answer":"要求。"}],"type":"既往史"},{"record":[{"id":null,"question":"还有其他不舒服吗？","display":"其他症状","answer":"七七。"}],"type":"其他症状"}]}}
    public class ResponseBaseQuestionRecordDataDTO
    {
        public List<ResponseBaseQuestionRecordDTO> recordList { get; set; }        
    }

    public class ResponseBaseQuestionRecordDTO
    {
        public string type { get; set; }

        public List<ResponseBaseQuestionAnswerDTO> record { get; set;}
            
    }

    public class ResponseBaseQuestionAnswerDTO
    {
        public string id { get; set; }

        public string question { get; set; }

        public string display { get; set; }

        public string answer { get; set; }
    }
}