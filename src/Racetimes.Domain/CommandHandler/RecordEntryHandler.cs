using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
{
    public class RecordEntryHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, RecordEntryCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, RecordEntryCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.AddEntry(command.EntryId, command.Discipline, command.Name, command.TimeInMillis);
            return Task.FromResult(executionResult);
        }
    }
}
