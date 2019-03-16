using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.Command
{
    public class CorrectEntryTimeCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public EntryId EntryId { get; private set; }
        public int TimeInMillis { get; private set; } = 0;

        public CorrectEntryTimeCommand(CompetitionId competitionId, EntryId entryId, int timeInMillis) : base(competitionId)
        {
            EntryId = entryId;
            TimeInMillis = timeInMillis;
        }
    }
}
