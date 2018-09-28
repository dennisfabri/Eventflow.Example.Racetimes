using System.Threading;
using System.Threading.Tasks;
using times.domain.Aggregate;
using times.domain.Command;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.CommandHandler
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
