using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleCategoryManager.Persistance.Services
{
    public abstract class ServiceBase
    {
        protected DataContextFactory _contextFactory;

        public ServiceBase(DataContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
    }
}
