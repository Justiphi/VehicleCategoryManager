using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleCategoryManager.Persistance
{
    public class DataContextFactory
    {
        private readonly DbContextOptions<DataContext> _options;

        public DataContextFactory(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        public DataContext CreateContext()
        {
            return new DataContext(_options);
        }
    }
}
