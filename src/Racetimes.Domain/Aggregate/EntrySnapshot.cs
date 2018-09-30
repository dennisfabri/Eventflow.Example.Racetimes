using EventFlow.Snapshots;
using System;
using Racetimes.Domain.Identity;

namespace Racetimes.Domain.Aggregate
{
    [Serializable]
    [SnapshotVersion("Entry", 1)]
    public class EntrySnapshot : ISnapshot
    {
        public EntrySnapshot(EntryId id, string name, string discipline, int timeInMillis)
        {
            Id = id;
            Name = name;
            Discipline = discipline;
            TimeInMillis = timeInMillis;
        }

        public EntryId Id { get; private set; }
        public string Name { get; private set; }
        public string Discipline { get; private set; }
        public int TimeInMillis { get; private set; }
    }
}
