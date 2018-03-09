using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthCloud.Consultation.Common
{
    public static class EmptyArray<T>
    {
        public static T[] Value { get; } = new T[0];
    }
}