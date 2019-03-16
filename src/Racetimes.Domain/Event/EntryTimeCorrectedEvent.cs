using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
{
    [EventVersion("EntryTimeCorrected", 1)]
    public class EntryTimeCorrectedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public EntryId EntryId { get; private set; }
        public int TimeInMillis { get; private set; }

        public EntryTimeCorrectedEvent(EntryId entryId, int timeInMillis)
        {
            EntryId = entryId;
            TimeInMillis = timeInMillis;
        }
    }
}
