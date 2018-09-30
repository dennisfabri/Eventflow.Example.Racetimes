using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
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
