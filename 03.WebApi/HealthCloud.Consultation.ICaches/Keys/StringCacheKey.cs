﻿using HealthCloud.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.ICaches.Keys
{


    public class StringCacheKey : ICacheKey
    {
        string _KeyId { get; set; }

        string _KeyType { get; set; }


        public StringCacheKey(Enums.StringCacheKeyType KeyType, string KeyId)
        {
            this._KeyType = KeyType.ToString();
            this._KeyId = KeyId;
        }
        public StringCacheKey(Enums.StringCacheKeyType KeyType)
        {
            this._KeyType = KeyType.ToString();
        }
        public string KeyName
        {
            get
            {
                return $"{_KeyType}:{_KeyId}";
            }
        }
    }
}
