using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
{
    public class ChangeEntryTimeHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, ChangeEntryTimeCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, ChangeEntryTimeCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.ChangeEntryTime(command.EntryId, command.TimeInMillis);
            return Task.FromResult(executionResult);
        }
    }
}
