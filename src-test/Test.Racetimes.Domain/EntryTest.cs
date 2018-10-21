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

namespace Test.Racetimes.Domain
{
    public class EntryTest
    {
        [Fact]
        public void AddEntryTest()
        {
            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(EntryAddedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(AddEntryCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(AddEntryHandler))
                .RegisterServices(sr => { sr.Register<IEntryLocator, EntryLocator>(); })
                .UseInMemoryReadStoreFor<CompetitionReadModel>()
                .UseInMemoryReadStoreFor<EntryReadModel, IEntryLocator>()
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
                const int time = 12345;

                // Rename
                executionResult = commandBus.Publish(new AddEntryCommand(domainId, entryId, discipline, competitor, time), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected magic number
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                // Verify that the read model has the expected magic number
                var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
                readModel2.AggregateId.Should().Be(entryId.Value);
                readModel2.Discipline.Should().Be(discipline);
                readModel2.Competitor.Should().Be(competitor);
                readModel2.TimeInMillis.Should().Be(time);
            }
        }

        [Fact]
        public void ChangeEntryTimeTest()
        {
            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(EntryAddedEvent), typeof(EntryTimeChangedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(AddEntryCommand), typeof(ChangeEntryTimeCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(AddEntryHandler), typeof(ChangeEntryTimeHandler))
                .RegisterServices(sr => { sr.Register<IEntryLocator, EntryLocator>(); })
                .UseInMemoryReadStoreFor<CompetitionReadModel>()
                .UseInMemoryReadStoreFor<EntryReadModel, IEntryLocator>()
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
                const int time1 = 12345;
                const int time2 = 54321;

                // Rename
                executionResult = commandBus.Publish(new AddEntryCommand(domainId, entryId, discipline, competitor, time1), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();

                // Verify that the read model has the expected magic number
                var readModel1 = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);
                readModel1.AggregateId.Should().Be(domainId.Value);
                readModel1.Competitionname.Should().Be(name);

                // Verify that the read model has the expected magic number
                var readModel2 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
                readModel2.AggregateId.Should().Be(entryId.Value);
                readModel2.Discipline.Should().Be(discipline);
                readModel2.Competitor.Should().Be(competitor);
                readModel2.TimeInMillis.Should().Be(time1);

                executionResult = commandBus.Publish(new ChangeEntryTimeCommand(domainId, entryId, time2), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                var readModel3 = queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(entryId), CancellationToken.None);
                readModel2.AggregateId.Should().Be(entryId.Value);
                readModel2.Discipline.Should().Be(discipline);
                readModel2.Competitor.Should().Be(competitor);
                readModel2.TimeInMillis.Should().Be(time2);
            }
        }

    }
}
