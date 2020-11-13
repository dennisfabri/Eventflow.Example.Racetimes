using EventFlow;
using EventFlow.Configuration;
using EventFlow.EntityFramework;
using EventFlow.EntityFramework.Extensions;
using EventFlow.Extensions;
using EventFlow.Queries;
using Racetimes.Domain.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.ReadModel.EntityFramework
{
    public static class ReadModelConfiguration
    {
        public static IEventFlowOptions AddEntityFrameworkReadModel(this IEventFlowOptions efo)
        {
            return efo
                .RegisterServices(sr => sr.Register<IEntryLocator, EntryLocator>())
                .UseEntityFrameworkReadModel<CompetitionReadModel, ExampleDbContext>()
                .UseEntityFrameworkReadModel<EntryReadModel, ExampleDbContext, IEntryLocator>()
                .AddQueryHandler<GetAllEntriesQueryHandler, GetAllEntriesQuery, EntryReadModel[]>()
                .ConfigureEntityFramework(EntityFrameworkConfiguration.New)
                .AddDbContextProvider<ExampleDbContext, DbContextProvider>();
        }

        public static async Task<CompetitionReadModel> Query(IRootResolver resolver, CompetitionId exampleId)
        {
            // Resolve the query handler and use the built-in query for fetching
            // read models by identity to get our read model representing the
            // state of our aggregate root
            return await resolver.Resolve<IQueryProcessor>().ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(exampleId), CancellationToken.None);
        }
    }
}