using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.Command
{
    public class ChangeEntryTimeCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public EntryId EntryId { get; private set; }
        public int TimeInMillis { get; private set; } = 0;

        public ChangeEntryTimeCommand(CompetitionId competitionId, EntryId entryId, int timeInMillis) : base(competitionId)
        {
            EntryId = entryId;
            TimeInMillis = timeInMillis;
        }
    }
}
