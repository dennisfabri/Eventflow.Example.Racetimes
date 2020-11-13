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
using Racetimes.Domain.Aggregate;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;
using Racetimes.ReadModel.MsSql;
using Racetimes.ReadModel.EntityFramework;
using EventFlow.Snapshots.Strategies;
using System.Configuration;

namespace Racetimes.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(CompetitionCorrectedEvent), typeof(CompetitionDeletedEvent), typeof(EntryRecordedEvent), typeof(EntryTimeCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(CorrectCompetitionCommand), typeof(DeleteCompetitionCommand), typeof(RecordEntryCommand), typeof(CorrectEntryTimeCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(CorrectCompetitionHandler), typeof(DeleteCompetitionHandler), typeof(RecordEntryHandler), typeof(CorrectEntryTimeHandler))
                .AddSnapshots(typeof(CompetitionSnapshot))
                .RegisterServices(sr => sr.Register(i => SnapshotEveryFewVersionsStrategy.Default))
                .RegisterServices(sr => sr.Register(c => ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString))
                .UseMssqlEventStore()
                .UseMsSqlSnapshotStore()
                .AddMsSqlReadModel()
                .AddEntityFrameworkReadModel()
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
                var executionResult = commandBus.PublishAsync(new RegisterCompetitionCommand(exampleId, user, name), CancellationToken.None).Result;

                executionResult = commandBus.PublishAsync(new CorrectCompetitionCommand(exampleId, name2), CancellationToken.None).Result;

                ReadModel.MsSql.ReadModelConfiguration.Query(resolver, exampleId).Wait();
                ReadModel.EntityFramework.ReadModelConfiguration.Query(resolver, exampleId).Wait();

                var entry1Id = EntryId.New;
                var entry2Id = EntryId.New;

                commandBus.PublishAsync(new RecordEntryCommand(exampleId, entry1Id, "Discipline 1", "Name 1", 11111), CancellationToken.None).Wait();
                commandBus.PublishAsync(new RecordEntryCommand(exampleId, entry2Id, "Discipline 2", "Name 2", 22222), CancellationToken.None).Wait();
                commandBus.PublishAsync(new CorrectEntryTimeCommand(exampleId, entry1Id, 10000), CancellationToken.None).Wait();
                commandBus.PublishAsync(new CorrectEntryTimeCommand(exampleId, entry2Id, 20000), CancellationToken.None).Wait();

                for (int x = 1; x < 100; x++)
                {
                    commandBus.PublishAsync(new CorrectEntryTimeCommand(exampleId, entry2Id, 2000 + x), CancellationToken.None).Wait();
                }

                commandBus.PublishAsync(new DeleteCompetitionCommand(exampleId), CancellationToken.None).Wait();
            }
            //Console.ReadLine();
        }
    }
}