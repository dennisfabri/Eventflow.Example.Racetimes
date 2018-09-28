using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace times.domain.Event
{
    [EventVersion("CompetitionDeleted", 1)]
    public class CompetitionDeletedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        private static readonly EntryId[] empty = new EntryId[0];
        public IList<EntryId> EntryIds { get; private set; }

        public CompetitionDeletedEvent(IEnumerable<EntryId> ids)
        {
            EntryIds = ids == null ? new List<EntryId>(empty) : new List<EntryId>(ids);
        }
    }
}
