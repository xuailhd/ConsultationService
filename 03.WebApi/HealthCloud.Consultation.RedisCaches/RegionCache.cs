using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.RedisCaches
{
    public class RegionCache
    {
        ///// <summary>
        ///// 获取地区树
        ///// </summary>
        ///// <param name="CreateCache">DB查找</param>
        ///// <returns></returns>
        //public ResponseRegionTreeDto GetRegionsTree(Func<ResponseRegionTreeDto> CreateCache)
        //{
        //    var cacheKey = new StringCacheKey(StringCacheKeyType.Sys_RegionTrees);

        //    var list = cacheKey.FromCache<ResponseRegionTreeDto>();
        //    if(list == null)
        //    {
        //        list = CreateCache();
        //        list.ToCache(cacheKey);
        //    }

        //    return list;
        //}

        //public List<ResponseFDRegionDto> GetAllRegions(Func<List<ResponseFDRegionDto>> CreateCache)
        //{
        //    var cacheKey = new StringCacheKey(StringCacheKeyType.Sys_Regions);

        //    var list = cacheKey.FromCache<List<ResponseFDRegionDto>>();
        //    if (list == null)
        //    {
        //        list = CreateCache();
        //        list.ToCache(cacheKey);
        //    }

        //    return list;
        //}
    }
}
