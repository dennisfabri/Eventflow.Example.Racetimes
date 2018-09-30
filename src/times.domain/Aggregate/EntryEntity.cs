using EventFlow.Entities;
using System;
using times.domain.Identity;

namespace times.domain.Aggregate
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

        public void ChangeTime(int timeInMillis)
        {
            TimeInMillis = timeInMillis;
        }

        public EntrySnapshot CreateSnapshot()
        {
            return new EntrySnapshot(Id, Name, Discipline, TimeInMillis);
        }
    }
}