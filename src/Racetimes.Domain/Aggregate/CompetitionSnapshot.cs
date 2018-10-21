using EventFlow.Snapshots;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Racetimes.Domain.Aggregate
{
    [Serializable]
    [SnapshotVersion("Competition", 1)]
    public class CompetitionSnapshot : ISnapshot
    {
        public CompetitionSnapshot(string competitionname, string user, bool isDeleted, params EntrySnapshot[] entries)
        {
            Competitionname = competitionname;
            User = user;
            IsDeleted = isDeleted;
            Entries = new ReadOnlyCollection<EntrySnapshot>(entries == null ? new EntrySnapshot[0] : entries);
        }

        public string Competitionname { get; }
        public string User { get; }
        public IReadOnlyCollection<EntrySnapshot> Entries { get; }
        public bool IsDeleted { get; }
    }
}
