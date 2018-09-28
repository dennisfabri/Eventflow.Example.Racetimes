using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.Command
{
    public class DeleteCompetitionCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public DeleteCompetitionCommand(CompetitionId id) : base(id)
        {
        }
    }
}
