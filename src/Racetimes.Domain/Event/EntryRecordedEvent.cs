using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
{
    [EventVersion("EntryRecorded", 1)]
    public class EntryRecordedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public EntryId EntryId { get; private set; }
        public string Name { get; private set; } = "";
        public string Discipline { get; private set; } = "";
        public int TimeInMillis { get; private set; } = 0;

        public EntryRecordedEvent(EntryId entryId, string discipline, string name, int timeInMillis)
        {
            EntryId = entryId;
            Discipline = discipline;
            Name = name;
            TimeInMillis = timeInMillis;
        }
    }
}
