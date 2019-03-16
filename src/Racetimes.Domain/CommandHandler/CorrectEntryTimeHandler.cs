using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
{
    public class CorrectEntryTimeHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, CorrectEntryTimeCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, CorrectEntryTimeCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.ChangeEntryTime(command.EntryId, command.TimeInMillis);
            return Task.FromResult(executionResult);
        }
    }
}
