using System.Threading;
using Racetimes.Domain.Command;
using Racetimes.Domain.CommandHandler;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.MsSql;
using EventFlow.MsSql.EventStores;
using EventFlow.MsSql.Extensions;
using EventFlow.Queries;
using Racetimes.Domain.Aggregate;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;
using Racetimes.ReadModel.MsSql;

namespace Racetimes.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionRenamedEvent), typeof(CompetitionDeletedEvent), typeof(EntryAddedEvent), typeof(EntryTimeChangedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(RenameCompetitionCommand), typeof(DeleteCompetitionCommand), typeof(AddEntryCommand), typeof(ChangeEntryTimeCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(RenameCompetitionHandler), typeof(DeleteCompetitionHandler), typeof(AddEntryHandler), typeof(ChangeEntryTimeHandler))
                .AddQueryHandler<GetAllEntriesQueryHandler, GetAllEntriesQuery, EntryReadModel[]>()
                .AddSnapshots(typeof(CompetitionSnapshot))
                .RegisterServices(sr => { sr.Register<IEntryLocator, EntryLocator>(); })
                .UseMssqlEventStore()
                .UseMsSqlSnapshotStore()
                .UseMssqlReadModel<CompetitionReadModel>()
                .UseMssqlReadModel<EntryReadModel, IEntryLocator>()
                .ConfigureMsSql(MsSqlConfiguration.New.SetConnectionString(@"Data Source=localhost;Initial Catalog=TimesEF;Integrated Security=SSPI;"))
                .CreateResolver())
            {

                var msSqlDatabaseMigrator = resolver.Resolve<IMsSqlDatabaseMigrator>();
                EventFlowEventStoresMsSql.MigrateDatabase(msSqlDatabaseMigrator);
                // var sql = EventFlowEventStoresMsSql.GetSqlScripts().Select(s => s.Content).ToArray();

                // Create a new identity for our aggregate root
                var exampleId = CompetitionId.New;

                // Define some important value
                const string name = "test-competition";
                const string name2 = "new-name";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(exampleId, user, name), CancellationToken.None);

                executionResult = commandBus.Publish(new RenameCompetitionCommand(exampleId, name2), CancellationToken.None);

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var exampleReadModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(exampleId), CancellationToken.None);

                var entry1Id = EntryId.New;
                var entry2Id = EntryId.New;

                executionResult = commandBus.Publish(new AddEntryCommand(exampleId, entry1Id, "Discipline 1", "Name 1", 11111), CancellationToken.None);
                executionResult = commandBus.Publish(new AddEntryCommand(exampleId, entry2Id, "Discipline 2", "Name 2", 22222), CancellationToken.None);
                executionResult = commandBus.Publish(new ChangeEntryTimeCommand(exampleId, entry1Id, 10000), CancellationToken.None);
                executionResult = commandBus.Publish(new ChangeEntryTimeCommand(exampleId, entry2Id, 20000), CancellationToken.None);

                for (int x = 1; x < 100; x++)
                {
                    executionResult = commandBus.Publish(new ChangeEntryTimeCommand(exampleId, entry2Id, 2000 + x), CancellationToken.None);
                }

                //executionResult = commandBus.Publish(new DeleteCompetitionCommand(exampleId), CancellationToken.None);
            }
            // Console.ReadLine();
        }
    }
}
