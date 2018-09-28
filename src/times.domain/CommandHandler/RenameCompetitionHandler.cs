using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using times.domain.Aggregate;
using times.domain.Command;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.CommandHandler
{
    public class RenameCompetitionHandler : CommandHandler<CompetitionAggregate, CompetitionId, IExecutionResult, RenameCompetitionCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(CompetitionAggregate aggregate, RenameCompetitionCommand command, CancellationToken cancellationToken)
        {
            var executionResult = aggregate.Rename(command.Name);
            return Task.FromResult(executionResult);
        }
    }
}
