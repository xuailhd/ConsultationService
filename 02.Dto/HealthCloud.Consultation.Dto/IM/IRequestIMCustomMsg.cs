﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.IM
{
    public interface IRequestIMCustomMsg<T>
    {
        T Data { get; set; }

        string Desc { get; set; }

        string Ext { get; }
    }
}
