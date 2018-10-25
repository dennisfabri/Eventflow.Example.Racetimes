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

namespace Test.Racetimes.Domain
{
    public class EntryTest
    {
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

        [Theory]
        [InlineData("Discipline", "Competitor", 12345, true)]
        [InlineData("", "Competitor", 12345, false)]
        [InlineData("Discipline", "", 12345, false)]
        [InlineData("Discipline", "Competitor", 0, false)]
        public void AddEntryTest(string discipline, string competitor, int time, bool expectedResult)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(EntryAddedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(AddEntryCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(AddEntryHandler))
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;
                var entryId = EntryId.New;

                // Define some important value
                const string name = "test-competition";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                // Preparation finished: Start with the test

                // Rename
                executionResult = commandBus.Publish(new AddEntryCommand(domainId, entryId, discipline, competitor, time), CancellationToken.None);
                executionResult.IsSuccess.Should().Be(expectedResult);

                // Verify that the read model has the expected value
                var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);

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
        public void ChangeEntryTimeTest(int time1, int time2, bool expectedResult)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(EntryAddedEvent), typeof(EntryTimeChangedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(AddEntryCommand), typeof(ChangeEntryTimeCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(AddEntryHandler), typeof(ChangeEntryTimeHandler))
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;
                var entryId = EntryId.New;

                // Define some important value
                const string name = "test-competition";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                const string discipline = "Discipline";
                const string competitor = "Competitor";

                // Rename
                executionResult = commandBus.Publish(new AddEntryCommand(domainId, entryId, discipline, competitor, time1), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                // Verify that the read model has the expected value
                var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
                readModel2.Should().NotBeNull();
                readModel2.AggregateId.Should().Be(entryId.Value);
                readModel2.Discipline.Should().Be(discipline);
                readModel2.Competitor.Should().Be(competitor);
                readModel2.TimeInMillis.Should().Be(time1);

                // Preparations finished: Start test

                executionResult = commandBus.Publish(new ChangeEntryTimeCommand(domainId, entryId, time2), CancellationToken.None);
                executionResult.IsSuccess.Should().Be(expectedResult);

                var readModel3 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
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
        public void ChangeEntryTimeOnMissingEntry()
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(EntryTimeChangedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(ChangeEntryTimeCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(ChangeEntryTimeHandler))
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;
                var entryId = EntryId.New;

                // Define some important value
                const string name = "test-competition";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                const int time = 12345;

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                // Preparations finished: Start test
                executionResult = commandBus.Publish(new ChangeEntryTimeCommand(domainId, entryId, time), CancellationToken.None);
                executionResult.IsSuccess.Should().BeFalse();

                var readModel3 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
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
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionDeletedEvent), typeof(EntryAddedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(DeleteCompetitionCommand), typeof(AddEntryCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(DeleteCompetitionHandler), typeof(AddEntryHandler))
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;

                // Define some important value
                const string name = "test-competition";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                const string discipline = "Discipline";

                EntryId[] ids = new EntryId[amount];

                for (int x = 0; x < amount; x++)
                {
                    var entryId = EntryId.New;
                    ids[x] = entryId;

                    string competitor = string.Format("Competitor {0}", x + 1);
                    int time = 12300 + x;

                    // Add
                    executionResult = commandBus.Publish(new AddEntryCommand(domainId, entryId, discipline, competitor, time), CancellationToken.None);
                    executionResult.IsSuccess.Should().BeTrue();

                    // Verify that the read model has the expected value
                    var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
                    readModel2.Should().NotBeNull();
                    readModel2.AggregateId.Should().Be(entryId.Value);
                    readModel2.Discipline.Should().Be(discipline);
                    readModel2.Competitor.Should().Be(competitor);
                    readModel2.TimeInMillis.Should().Be(time);
                }

                // Preparation finished: Start with the test

                // Delete
                executionResult = commandBus.Publish(new DeleteCompetitionCommand(domainId), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                foreach (var entryId in ids)
                {
                    // Verify that the read model has the expected value
                    var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
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
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionDeletedEvent), typeof(EntryAddedEvent), typeof(EntryTimeChangedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(DeleteCompetitionCommand), typeof(AddEntryCommand), typeof(ChangeEntryTimeCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(DeleteCompetitionHandler), typeof(AddEntryHandler), typeof(ChangeEntryTimeHandler))
                .AddSnapshots(typeof(CompetitionSnapshot))
                .RegisterServices(sr => sr.Register(i => SnapshotEveryFewVersionsStrategy.With(1)))
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;

                // Define some important value
                const string name = "test-competition";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected value
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.Should().NotBeNull();
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                const string discipline = "Discipline";

                EntryId[] ids = new EntryId[amount];

                for (int x = 0; x < amount; x++)
                {
                    var entryId = EntryId.New;
                    ids[x] = entryId;

                    string competitor = string.Format("Competitor {0}", x + 1);
                    int time = 12300 + x;

                    // Add
                    executionResult = commandBus.Publish(new AddEntryCommand(domainId, entryId, discipline, competitor, time + 1), CancellationToken.None);
                    executionResult.IsSuccess.Should().BeTrue();

                    // Change time
                    executionResult = commandBus.Publish(new ChangeEntryTimeCommand(domainId, entryId, time), CancellationToken.None);
                    executionResult.IsSuccess.Should().BeTrue();

                    // Verify that the read model has the expected value
                    var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
                    readModel2.Should().NotBeNull();
                    readModel2.AggregateId.Should().Be(entryId.Value);
                    readModel2.Discipline.Should().Be(discipline);
                    readModel2.Competitor.Should().Be(competitor);
                    readModel2.TimeInMillis.Should().Be(time);
                }
            }
        }
    }
}