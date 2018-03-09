using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HealthCloud.Consultation.WebApi.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ReturnModelDescriptionAttribute : DescriptionAttribute
    {
        public string Property { get; }

        public ReturnModelDescriptionAttribute(string description) : base(description)
        {
        }

        public ReturnModelDescriptionAttribute(string property, string description) : base(description)
        {
            Property = property;
        }
    }
}