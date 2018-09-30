using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
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
