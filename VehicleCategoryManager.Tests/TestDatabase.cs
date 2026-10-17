using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using VehicleCategoryManager.Persistance;

namespace VehicleCategoryManager.Tests
{

    public class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;

        public DataContextFactory Factory { get; }

        public DataContext Context { get; }


        public TestDatabase()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(_connection)
                .Options;

            Factory = new DataContextFactory(options);

            Context = Factory.CreateContext();

            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }

}
