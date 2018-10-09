using EventFlow.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Racetimes.ReadModel.EntityFramework
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        {
        }

        public DbSet<CompetitionReadModel> Competitions { get; set; }
        public DbSet<EntryReadModel> Entries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AddEventFlowEvents()
                .AddEventFlowSnapshots();
        }
    }
}
