using times.domain.Aggregate;
using times.domain.Event;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace times.domain.Readmodel
{
    public class CompetitionReadModel : VersionedReadModel,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionCreatedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionRenamedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent>
    {
        public string Competitionname { get; private set; }
        public string Username { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionCreatedEvent> domainEvent)
        {
            Update();
            Competitionname = domainEvent.AggregateEvent.Name;
            Username = domainEvent.AggregateEvent.User;
            AggregateId = domainEvent.AggregateIdentity.Value;
        }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionRenamedEvent> domainEvent)
        {
            Update();
            Competitionname = domainEvent.AggregateEvent.Name;
        }
        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent> domainEvent)
        {
            context.MarkForDeletion();
        }
    }
}
