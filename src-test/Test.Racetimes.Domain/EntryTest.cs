using System.Threading;
using Racetimes.Domain.Command;
using Racetimes.Domain.CommandHandler;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.Queries;
using FluentAssertions;
using Xunit;
using Racetimes.ReadModel.MsSql;
using Test.Racetimes.Domain.Extension;
using Racetimes.ReadModel;
using Racetimes.Domain.Aggregate;
using EventFlow.Snapshots.Strategies;
using EventFlow.Configuration;

namespace Test.Racetimes.Domain
{
    public class EntryTest
    {
        #region Helpers

        private static IEventFlowOptions New
        {
            get {
                return EventFlowOptions.New
                    .RegisterServices(sr => sr.Register(i => SnapshotNeverStrategy.Default))
                    .RegisterServices(sr => sr.Register<IEntryLocator, EntryLocator>())
                    .UseInMemoryReadStoreFor<CompetitionReadModel>()
                    .UseInMemoryReadStoreFor<EntryReadModel, IEntryLocator>();
            }
        }

        private static CompetitionId PrepareCompetition(EventFlow.Configuration.IRootResolver resolver, string name, string user, CompetitionId competitionId = null)
        {
            var domainId = competitionId ?? CompetitionId.New;

            // Define some important value
            // Resolve the command bus and use it to publish a command
            var commandBus = resolver.Resolve<ICommandBus>();

            // Create
            var executionResult = commandBus.PublishAsync(new RegisterCompetitionCommand(domainId, user, name), CancellationToken.None).Result;
            executionResult.IsSuccess.Should().BeTrue();

            // Resolve the query handler and use the built-in query for fetching
            // read models by identity to get our read model representing the
            // state of our aggregate root
            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            // Verify that the read model has the expected value
            var readModel1 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;
            readModel1.Should().NotBeNull();
            readModel1.AggregateId.Should().Be(domainId.Value);
            readModel1.Competitionname.Should().Be(name);
            readModel1.Username.Should().Be(user);

            return domainId;
        }

        #endregion

        [Theory]
        [InlineData("Discipline", "Competitor", 12345, true)]
        [InlineData("", "Competitor", 12345, false)]
        [InlineData("Discipline", "", 12345, false)]
        [InlineData("Discipline", "Competitor", 0, false)]
        public void RecordEntryTest(string discipline, string competitor, int time, bool expectedResult)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(EntryRecordedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(RecordEntryCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(RecordEntryHandler))
                .CreateResolver())
            {
                var domainId = PrepareCompetition(resolver, "test-competition", "test-user");
                var entryId = EntryId.New;

                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var commandBus = resolver.Resolve<ICommandBus>();

                // Preparation finished: Start with the test

                // Rename
                var executionResult = commandBus.PublishAsync(new RecordEntryCommand(domainId, entryId, discipline, competitor, time), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().Be(expectedResult);

                // Verify that the read model has the expected value
                var readModel2 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None).Result;

                if (expectedResult)
                {
                    readModel2.Should().NotBeNull();
                    readModel2.AggregateId.Should().Be(entryId.Value);
                    readModel2.Discipline.Should().Be(discipline);
                    readModel2.Competitor.Should().Be(competitor);
                    readModel2.TimeInMillis.Should().Be(time);
                }
                else
                {
                    readModel2.Should().BeNull();
                }
            }
        }

        [Theory]
        [InlineData(12345, 12346, true)]
        [InlineData(12345, 12345, false)]
        [InlineData(12345, 0, false)]
        public void CorrectEntryTimeTest(int time1, int time2, bool expectedResult)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(EntryRecordedEvent), typeof(EntryTimeCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(RecordEntryCommand), typeof(CorrectEntryTimeCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(RecordEntryHandler), typeof(CorrectEntryTimeHandler))
                .CreateResolver())
            {
                const string name = "test-competition";
                const string user = "test-user";

                var domainId = PrepareCompetition(resolver, name, user);
                var entryId = EntryId.New;

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                const string discipline = "Discipline";
                const string competitor = "Competitor";

                // Rename
                var executionResult = commandBus.PublishAsync(new RecordEntryCommand(domainId, entryId, discipline, competitor, time1), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);
                readModel1.Username.Should().Be(user);

                // Verify that the read model has the expected value
                var readModel2 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None).Result;
                readModel2.Should().NotBeNull();
                readModel2.AggregateId.Should().Be(entryId.Value);
                readModel2.Discipline.Should().Be(discipline);
                readModel2.Competitor.Should().Be(competitor);
                readModel2.TimeInMillis.Should().Be(time1);

                // Preparations finished: Start test

                executionResult = commandBus.PublishAsync(new CorrectEntryTimeCommand(domainId, entryId, time2), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().Be(expectedResult);

                var readModel3 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None).Result;
                readModel3.Should().NotBeNull();
                readModel3.AggregateId.Should().Be(entryId.Value);
                readModel3.Discipline.Should().Be(discipline);
                readModel3.Competitor.Should().Be(competitor);
                if (expectedResult)
                {
                    readModel3.TimeInMillis.Should().Be(time2);
                }
                else
                {
                    readModel3.TimeInMillis.Should().Be(time1);
                }
            }
        }
        [Fact]
        public void CorrectEntryTimeOnMissingEntry()
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(EntryTimeCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(CorrectEntryTimeCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(CorrectEntryTimeHandler))
                .CreateResolver())
            {
                const string name = "test-competition";
                const string user = "test-user";

                var domainId = PrepareCompetition(resolver, name, user);
                var entryId = EntryId.New;

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                const int time = 12345;

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                // Preparations finished: Start test
                var executionResult = commandBus.PublishAsync(new CorrectEntryTimeCommand(domainId, entryId, time), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeFalse();

                var readModel3 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None).Result;
                readModel3.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public void DeleteCompetitionTest(int amount)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(CompetitionDeletedEvent), typeof(EntryRecordedEvent), typeof(EntryTimeCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(DeleteCompetitionCommand), typeof(RecordEntryCommand), typeof(CorrectEntryTimeCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(DeleteCompetitionHandler), typeof(RecordEntryHandler), typeof(CorrectEntryTimeHandler))
                .CreateResolver())
            {
                var domainId = CompetitionId.New;
                EntryId[] ids = RegisterCompetitionWithEntries(resolver, amount, domainId);

                var commandBus = resolver.Resolve<ICommandBus>();
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Delete
                var executionResult = commandBus.PublishAsync(new DeleteCompetitionCommand(domainId), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                foreach (var entryId in ids)
                {
                    // Verify that the read model has the expected value
                    var readModel2 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None).Result;
                    readModel2.Should().BeNull();
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        public void CreateSnapshotTest(int amount)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(CompetitionDeletedEvent), typeof(EntryRecordedEvent), typeof(EntryTimeCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(DeleteCompetitionCommand), typeof(RecordEntryCommand), typeof(CorrectEntryTimeCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(DeleteCompetitionHandler), typeof(RecordEntryHandler), typeof(CorrectEntryTimeHandler))
                .AddSnapshots(typeof(CompetitionSnapshot))
                .RegisterServices(sr => sr.Register(i => SnapshotEveryFewVersionsStrategy.With(1)))
                .CreateResolver())
            {
                RegisterCompetitionWithEntries(resolver, amount);
            }
        }

        private EntryId[] RegisterCompetitionWithEntries(IRootResolver resolver, int amount, CompetitionId competitionId = null)
        {
            var domainId = PrepareCompetition(resolver, "test-competition", "test-user", competitionId);

            // Resolve the command bus and use it to publish a command
            var commandBus = resolver.Resolve<ICommandBus>();

            // Resolve the query handler and use the built-in query for fetching
            // read models by identity to get our read model representing the
            // state of our aggregate root
            var queryProcessor = resolver.Resolve<IQueryProcessor>();

            const string discipline = "Discipline";

            EntryId[] ids = new EntryId[amount];

            for (int x = 0; x < amount; x++)
            {
                var entryId = EntryId.New;
                ids[x] = entryId;

                string competitor = string.Format("Competitor {0}", x + 1);
                int time = 12300 + x;

                // Add
                var executionResult = commandBus.PublishAsync(new RecordEntryCommand(domainId, entryId, discipline, competitor, time + 1), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Change time
                executionResult = commandBus.PublishAsync(new CorrectEntryTimeCommand(domainId, entryId, time), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Verify that the read model has the expected value
                var readModel2 = queryProcessor.ProcessAsync(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None).Result;
                readModel2.Should().NotBeNull();
                readModel2.AggregateId.Should().Be(entryId.Value);
                readModel2.Discipline.Should().Be(discipline);
                readModel2.Competitor.Should().Be(competitor);
                readModel2.TimeInMillis.Should().Be(time);
            }
            return ids;
        }
    }
}