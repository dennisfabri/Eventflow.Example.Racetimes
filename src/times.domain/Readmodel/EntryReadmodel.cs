using times.domain.Aggregate;
using times.domain.Event;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace times.domain.Readmodel
{
    public class EntryReadModel : VersionedReadModel,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, EntryAddedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, EntryTimeChangedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent>
    {
        public string Discipline { get; private set; }
        public string Competitor { get; private set; }
        public int TimeInMillis { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, EntryAddedEvent> domainEvent)
        {
            Update();
            Competitor = domainEvent.AggregateEvent.Name;
            Discipline = domainEvent.AggregateEvent.Discipline;
            TimeInMillis = domainEvent.AggregateEvent.TimeInMillis;
            AggregateId = domainEvent.AggregateEvent.EntryId.Value;
        }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, EntryTimeChangedEvent> domainEvent)
        {
            Update();
            TimeInMillis = domainEvent.AggregateEvent.TimeInMillis;
        }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent> domainEvent)
        {
            context.MarkForDeletion();
        }
    }
}
