using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
{
    public class DeleteCompetitionHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, DeleteCompetitionCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, DeleteCompetitionCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.Delete();
            return Task.FromResult(executionResult);
        }
    }
}
