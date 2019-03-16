using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.Command
{
    public class CorrectCompetitionCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public string Name { get; private set; }

        public CorrectCompetitionCommand(CompetitionId id, string name) : base(id)
        {
            Name = name;
        }
    }
}
