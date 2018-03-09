using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Repositories
{
    public interface IBaseRepository
    {
        DBEntities Db { get; set; }
    }
}
