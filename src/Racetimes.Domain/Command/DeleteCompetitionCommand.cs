using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.Command
{
    public class DeleteCompetitionCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public DeleteCompetitionCommand(CompetitionId id) : base(id)
        {
        }
    }
}
