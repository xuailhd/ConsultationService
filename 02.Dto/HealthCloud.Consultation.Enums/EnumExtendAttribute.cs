using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Enums
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumExtendAttribute : Attribute
    {
        public EnumExtendAttribute(params Type[] extendTypes)
        {
            ExtendTypes = extendTypes;
        }

        public Type[] ExtendTypes { get; }
    }
}