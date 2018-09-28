using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.Command
{
    public class RenameCompetitionCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public string Name { get; private set; }

        public RenameCompetitionCommand(CompetitionId id, string name) : base(id)
        {
            Name = name;
        }
    }
}
