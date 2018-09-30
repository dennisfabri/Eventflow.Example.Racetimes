using EventFlow.Snapshots;
using System;

namespace times.domain.Aggregate
{
    [Serializable]
    [SnapshotVersion("Competition", 1)]
    public class CompetitionSnapshot : ISnapshot
    {
        public string Competitionname { get; set; } = "";
        public string User { get; set; } = "";
        public EntrySnapshot[] Entries { get; set; } = new EntrySnapshot[0];
        public bool IsDeleted { get; set; } = false;
    }
}
