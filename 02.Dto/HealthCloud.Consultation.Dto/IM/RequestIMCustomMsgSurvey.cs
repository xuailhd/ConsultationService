using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.IM
{
    /// <summary>
    /// 单选题
    /// </summary>
    public class RadioTopic
    {
        public string Type { get { return "RadioTopic"; } }

        public List<string> Answer { get; set; }
    }

    public class RequestIMCustomMsgSurvey : IRequestIMCustomMsg<RadioTopic>
    {
        public RadioTopic Data
        {
            get; set;
        }

        public string Desc
        {
            get; set;
        }

        public string Ext
        {
            get
            {
                return EnumIMCustomMsgType.Survey_Question.GetEnumDescript();
            }
        }
    }
}
