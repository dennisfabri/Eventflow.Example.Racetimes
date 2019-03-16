using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace Racetimes.ReadModel.EntityFramework
{
    public class CompetitionReadModel : VersionedReadModel, IAmReadModelForCompetitionAggregate
    {
        public string Competitionname { get; private set; }
        public string Username { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionRegisteredEvent> domainEvent)
        {
            Competitionname = domainEvent.AggregateEvent.Name;
            Username = domainEvent.AggregateEvent.User;
            Id = domainEvent.AggregateIdentity.Value;
        }

        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionCorrectedEvent> domainEvent)
        {
            Competitionname = domainEvent.AggregateEvent.Name;
        }
        public void Apply(IReadModelContext context, IDomainEvent<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent> domainEvent)
        {
            context.MarkForDeletion();
        }
    }
}
