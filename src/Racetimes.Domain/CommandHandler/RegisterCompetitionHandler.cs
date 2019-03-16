using System.Threading;
using System.Threading.Tasks;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.CommandHandler
{
    public class RegisterCompetitionHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, RegisterCompetitionCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, RegisterCompetitionCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.Create(command.User, command.Name);
            return Task.FromResult(executionResult);
        }
    }
}
