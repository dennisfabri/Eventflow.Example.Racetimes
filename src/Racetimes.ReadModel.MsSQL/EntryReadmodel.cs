using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace Racetimes.ReadModel.MsSql
{
    public class EntryReadModel : VersionedReadModel, IAmReadModelForEntryEntity
    {
        public string Discipline { get; private set; }
        public string Competitor { get; private set; }
        public int TimeInMillis { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, EntryAddedEvent> domainEvent)
        {
            Competitor = domainEvent.AggregateEvent.Name;
            Discipline = domainEvent.AggregateEvent.Discipline;
            TimeInMillis = domainEvent.AggregateEvent.TimeInMillis;
            AggregateId = domainEvent.AggregateEvent.EntryId.Value;
        }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, EntryTimeChangedEvent> domainEvent)
        {
            TimeInMillis = domainEvent.AggregateEvent.TimeInMillis;
        }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent> domainEvent)
        {
            context.MarkForDeletion();
        }
    }
}
