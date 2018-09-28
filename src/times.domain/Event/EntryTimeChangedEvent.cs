using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace times.domain.Event
{
    [EventVersion("EntryTimeChanged", 1)]
    public class EntryTimeChangedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public EntryId EntryId { get; private set; }
        public int TimeInMillis { get; private set; }

        public EntryTimeChangedEvent(EntryId entryId, int timeInMillis)
        {
            EntryId = entryId;
            TimeInMillis = timeInMillis;
        }
    }
}
