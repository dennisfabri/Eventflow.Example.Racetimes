using EventFlow.Aggregates;
using System.Collections.Generic;
using System.Linq;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;

namespace Racetimes.ReadModel.MsSql
{
    public class EntryLocator : IEntryLocator
    {
        public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
        {            
            var entryAdded = domainEvent as IDomainEvent<CompetitionAggregate, CompetitionId, EntryAddedEvent>;
            if (entryAdded != null)
            {
                yield return entryAdded.AggregateEvent.EntryId.Value;
            }
            var entryChanged = domainEvent as IDomainEvent<CompetitionAggregate, CompetitionId, EntryTimeChangedEvent>;
            if (entryChanged != null)
            {
                yield return entryChanged.AggregateEvent.EntryId.Value;
            }
            var competitionDeleted = domainEvent as IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent>;
            if (competitionDeleted != null)
            {
                foreach (var id in competitionDeleted.AggregateEvent.EntryIds)
                {
                    yield return id.Value;
                }
            }
            yield break;
        }
    }
}
