﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Dto.Common
{
    /// <summary>
    /// 分页请求
    /// </summary>
    public interface IPagerRequest
    {
        /// <summary>
        /// 分页大小
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 分页索引
        /// </summary>
        int CurrentPage { get; set; }
    }
}

