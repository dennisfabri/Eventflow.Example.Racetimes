using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.Command
{
    public class CreateCompetitionCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public string User { get; private set; }
        public string Name { get; private set; }

        public CreateCompetitionCommand(CompetitionId id, string user, string name) : base(id)
        {
            User = user;
            Name = name;
        }
    }
}
