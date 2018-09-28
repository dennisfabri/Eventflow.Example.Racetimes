using System.Threading;
using System.Threading.Tasks;
using times.domain.Aggregate;
using times.domain.Command;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.CommandHandler
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
