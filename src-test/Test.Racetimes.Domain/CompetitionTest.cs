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
using EventFlow.Snapshots.Strategies;
using Racetimes.Domain.Aggregate;

namespace Test.Racetimes.Domain
{
    public class CompetitionTest
    {
        [Theory]
        [InlineData("Test competition", "Test user", true)]
        [InlineData("Test competition", "", false)]
        [InlineData("Test competition", null, false)]
        [InlineData("", "Test user", false)]
        [InlineData(null, "Test user", false)]
        [InlineData("", "", false)]
        [InlineData(null, "", false)]
        [InlineData("", null, false)]
        [InlineData(null, null, false)]
        public void CreateTest(string name, string user, bool expectedResult)
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

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name), CancellationToken.None);
                executionResult.IsSuccess.Should().Be(expectedResult);

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                if (expectedResult)
                {
                    // Verify that the read model has the expected values
                    readModel.Should().NotBeNull();
                    readModel.AggregateId.Should().Be(domainId.Value);
                    readModel.Competitionname.Should().Be(name);
                }
                else
                {
                    readModel.Should().BeNull();
                }
            }
        }

        [Theory]
        [InlineData("competition 1", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void RenameTest(string name2, bool expectedResult)
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
                const string user = "test-user";
                const string name1 = "test-competition";
                const string emptyName = "";
                const string nullName = null;

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name1), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Rename
                executionResult = commandBus.Publish(new RenameCompetitionCommand(domainId, name2), CancellationToken.None);
                executionResult.IsSuccess.Should().Be(expectedResult);

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                // Verify that the read model has the expected values
                readModel.Should().NotBeNull();
                readModel.AggregateId.Should().Be(domainId.Value);
                readModel.Competitionname.Should().Be(expectedResult ? name2 : name1);
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

                // Readmodel has been deleted
                readModel.Should().BeNull();

                // Cannot delete twice
                executionResult = commandBus.Publish(new DeleteCompetitionCommand(domainId), CancellationToken.None);
                executionResult.IsSuccess.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData("competition 1")]
        [InlineData("competition 1", "competition 2")]
        [InlineData("competition 1", "competition 2", "competition 3")]
        [InlineData("competition 1", "competition 2", "competition3", "competition 4")]
        public void CreateSnapshotTest(params string[] names)
        {
            using (var resolver = EventFlowOptions.New
                .AddEvents(typeof(CompetitionCreatedEvent), typeof(CompetitionRenamedEvent))
                .AddCommands(typeof(CreateCompetitionCommand), typeof(RenameCompetitionCommand))
                .AddCommandHandlers(typeof(CreateCompetitionHandler), typeof(RenameCompetitionHandler))
                .AddSnapshots(typeof(CompetitionSnapshot))
                .RegisterServices(sr => sr.Register(i => SnapshotEveryFewVersionsStrategy.With(1)))
                .UseInMemoryReadStoreFor<CompetitionReadModel>()
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;

                // Define some important value
                const string user = "test-user";
                const string name1 = "test-competition";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.Publish(new CreateCompetitionCommand(domainId, user, name1), CancellationToken.None);
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                // Verify that the read model has the expected values
                readModel.Should().NotBeNull();
                readModel.AggregateId.Should().Be(domainId.Value);
                readModel.Competitionname.Should().Be(name1);

                foreach (string name2 in names)
                {
                    // Rename
                    executionResult = commandBus.Publish(new RenameCompetitionCommand(domainId, name2), CancellationToken.None);
                    executionResult.IsSuccess.Should().BeTrue();

                    readModel = queryProcessor.Process(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None);

                    // Verify that the read model has the expected values
                    readModel.AggregateId.Should().Be(domainId.Value);
                    readModel.Competitionname.Should().Be(name2);
                }
            }
        }
    }
}
