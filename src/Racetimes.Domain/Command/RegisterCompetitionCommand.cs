using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.Command
{
    public class RegisterCompetitionCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public string User { get; private set; }
        public string Name { get; private set; }

        public RegisterCompetitionCommand(CompetitionId id, string user, string name) : base(id)
        {
            User = user;
            Name = name;
        }
    }
}
