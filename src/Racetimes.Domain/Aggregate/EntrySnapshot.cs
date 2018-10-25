using EventFlow.Snapshots;
using System;
using Racetimes.Domain.Identity;

namespace Racetimes.Domain.Aggregate
{
    [Serializable]
    [SnapshotVersion("Entry", 1)]
    public class EntrySnapshot
    {
        public EntrySnapshot(EntryId id, string name, string discipline, int timeInMillis)
        {
            Id = id;
            Name = name;
            Discipline = discipline;
            TimeInMillis = timeInMillis;
        }

        public EntryId Id { get; }
        public string Name { get; }
        public string Discipline { get; }
        public int TimeInMillis { get; }
    }
}
