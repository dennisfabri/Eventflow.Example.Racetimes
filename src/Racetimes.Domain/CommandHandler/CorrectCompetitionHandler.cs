using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
{
    public class CorrectCompetitionHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, CorrectCompetitionCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, CorrectCompetitionCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.Rename(command.Name);
            return Task.FromResult(executionResult);
        }
    }
}
