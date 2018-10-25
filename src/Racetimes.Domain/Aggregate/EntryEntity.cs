using EventFlow.Aggregates;
using EventFlow.Entities;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;

namespace Racetimes.Domain.Aggregate
{
    public class EntryEntity : Entity<EntryId>
    {
        public string Name { get; private set; } = "";
        public string Discipline { get; private set; } = "";
        public int TimeInMillis { get; private set; } = 0;

        public EntryEntity(EntryId id, string discipline, string name, int timeInMillis) : base(id)
        {
            Discipline = discipline;
            Name = name;
            TimeInMillis = timeInMillis;
        }

        public EntryEntity(EntrySnapshot es) : this(es.Id, es.Discipline, es.Name, es.TimeInMillis) { }

        internal IAggregateEvent<CompetitionAggregate, CompetitionId> ChangeTime(int timeInMillis)
        {
            if (timeInMillis <= 0)
            {
                return null;
            }
            if (TimeInMillis == timeInMillis)
            {
                return null;
            }
            return new EntryTimeChangedEvent(Id, timeInMillis);
        }

        internal void Apply(int timeInMillis)
        {
            TimeInMillis = timeInMillis;
        }

        public EntrySnapshot CreateSnapshot()
        {
            return new EntrySnapshot(Id, Name, Discipline, TimeInMillis);
        }
    }
}