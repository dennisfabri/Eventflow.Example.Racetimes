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

namespace Test.Racetimes.Domain
{
    public class CompetitionTest
    {
        [Fact]
        public void CreateTest()
        {
            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionRenamedEvent))
                .AddCommands(typeof(CreateCompetitionCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler))
                .RegisterServices(sr => sr.Register(i => SnapshotNeverStrategy.Default))
                .UseInMemoryReadStoreFor<CompetitionReadModel>()
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
                var readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                // Verify that the read model has the expected magic number
                readModel.AggregateId.Should().Be(domainId.Value);
                readModel.Competitionname.Should().Be(name);
            }
        }

        [Fact]
        public void RenameTest()
        {
            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionRenamedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(RenameCompetitionCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(RenameCompetitionHandler))
                .RegisterServices(sr => sr.Register(i => SnapshotNeverStrategy.Default))
                .UseInMemoryReadStoreFor<CompetitionReadModel>()
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;

                // Define some important value
                const string name = "test-competition";
                const string user = "test-user";
                const string name2 = "new-competition";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Rename
                executionResult = commandBus.Publish(new RenameCompetitionCommand(domainId, name2), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                // Verify that the read model has the expected magic number
                readModel.AggregateId.Should().Be(domainId.Value);
                readModel.Competitionname.Should().Be(name2);
            }
        }

        [Fact]
        public void DeleteTest()
        {
            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionDeletedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(DeleteCompetitionCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(DeleteCompetitionHandler))
                .RegisterServices(sr => sr.Register(i => SnapshotNeverStrategy.Default))
                .UseInMemoryReadStoreFor<CompetitionReadModel>()
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

                // Rename
                executionResult = commandBus.Publish(new DeleteCompetitionCommand(domainId), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                // Verify that the read model has the expected magic number
                readModel.Should().BeNull();
            }
        }

        [Fact]
        public void CreateSnapshotTest()
        {
            // Todo: Determine a way of testing this
        }
    }
}
