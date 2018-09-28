using System.Threading;
using System.Threading.Tasks;
using times.domain.Aggregate;
using times.domain.Command;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.CommandHandler
{
    public class CreateCompetitionHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, CreateCompetitionCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, CreateCompetitionCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.Create(command.User, command.Name);
            return Task.FromResult(executionResult);
        }
    }
}
