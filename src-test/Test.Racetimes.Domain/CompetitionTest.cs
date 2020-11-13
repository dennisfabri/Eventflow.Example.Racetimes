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
        private static IEventFlowOptions New
        {
            get {
                return EventFlowOptions.New
                    .RegisterServices(sr => sr.Register(i => SnapshotNeverStrategy.Default))
                    .UseInMemoryReadStoreFor<CompetitionReadModel>();
            }
        }

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
        public void RegisterTest(string name, string user, bool expectedResult)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent))
                .AddCommands(typeof(RegisterCompetitionCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler))
                .CreateResolver())
            {
                // Create a new identity for our aggregate root
                var domainId = CompetitionId.New;

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();

                // Create
                var executionResult = commandBus.PublishAsync(new RegisterCompetitionCommand(domainId, user, name), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().Be(expectedResult);

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;

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
        [InlineData("test-competition", false)]        
        [InlineData("", false)]
        [InlineData(null, false)]
        public void CorrectTest(string name2, bool expectedResult)
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(CompetitionCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(CorrectCompetitionCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(CorrectCompetitionHandler))
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
                var executionResult = commandBus.PublishAsync(new RegisterCompetitionCommand(domainId, user, name1), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Rename
                executionResult = commandBus.PublishAsync(new CorrectCompetitionCommand(domainId, name2), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().Be(expectedResult);

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;

                // Verify that the read model has the expected values
                readModel.Should().NotBeNull();
                readModel.AggregateId.Should().Be(domainId.Value);
                readModel.Competitionname.Should().Be(expectedResult ? name2 : name1);
            }
        }

        [Fact]
        public void DeleteTest()
        {
            using (var resolver = New
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(CompetitionDeletedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(DeleteCompetitionCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(DeleteCompetitionHandler))
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
                var executionResult = commandBus.PublishAsync(new RegisterCompetitionCommand(domainId, user, name), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Delete
                executionResult = commandBus.PublishAsync(new DeleteCompetitionCommand(domainId), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;

                // Readmodel has been deleted
                readModel.Should().BeNull();

                // Cannot delete twice
                executionResult = commandBus.PublishAsync(new DeleteCompetitionCommand(domainId), CancellationToken.None).Result;
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
                .AddEvents(typeof(CompetitionRegisteredEvent), typeof(CompetitionCorrectedEvent))
                .AddCommands(typeof(RegisterCompetitionCommand), typeof(CorrectCompetitionCommand))
                .AddCommandHandlers(typeof(RegisterCompetitionHandler), typeof(CorrectCompetitionHandler))
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
                var executionResult = commandBus.PublishAsync(new RegisterCompetitionCommand(domainId, user, name1), CancellationToken.None).Result;
                executionResult.IsSuccess.Should().BeTrue();

                // Resolve the query handler and use the built-in query for fetching
                // read models by identity to get our read model representing the
                // state of our aggregate root
                var queryProcessor = resolver.Resolve<IQueryProcessor>();
                var readModel = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;

                // Verify that the read model has the expected values
                readModel.Should().NotBeNull();
                readModel.AggregateId.Should().Be(domainId.Value);
                readModel.Competitionname.Should().Be(name1);

                foreach (string name2 in names)
                {
                    // Rename
                    executionResult = commandBus.PublishAsync(new CorrectCompetitionCommand(domainId, name2), CancellationToken.None).Result;
                    executionResult.IsSuccess.Should().BeTrue();

                    readModel = queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(domainId), CancellationToken.None).Result;

                    // Verify that the read model has the expected values
                    readModel.AggregateId.Should().Be(domainId.Value);
                    readModel.Competitionname.Should().Be(name2);
                }
            }
        }
    }
}