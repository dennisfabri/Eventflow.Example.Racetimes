using EventFlow.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;

namespace Racetimes.ReadModel.EntityFramework
{
    public class DbContextProvider : IDbContextProvider<ExampleDbContext>, IDisposable
    {
        private readonly DbContextOptions<ExampleDbContext> _options;

        public DbContextProvider(string msSqlConnectionString)
        {
            _options = new DbContextOptionsBuilder<ExampleDbContext>()
                .UseSqlServer(@"Data Source=localhost;Initial Catalog=TimesEF;Integrated Security=SSPI;")
                .Options;
        }

        public ExampleDbContext CreateContext()
        {
            var context = new ExampleDbContext(_options);
            context.Database.EnsureCreated();
            return context;
        }

        public void Dispose()
        {
        }
    }


}
