using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace times.domain.Event
{
    [EventVersion("EntryAdded", 1)]
    public class EntryAddedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public EntryId EntryId { get; private set; }
        public string Name { get; private set; } = "";
        public string Discipline { get; private set; } = "";
        public int TimeInMillis { get; private set; } = 0;

        public EntryAddedEvent(EntryId entryId, string discipline, string name, int timeInMillis)
        {
            EntryId = entryId;
            Discipline = discipline;
            Name = name;
            TimeInMillis = timeInMillis;
        }
    }
}
