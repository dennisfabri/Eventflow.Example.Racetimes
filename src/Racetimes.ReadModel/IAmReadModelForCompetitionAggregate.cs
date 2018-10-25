using EventFlow.ReadStores;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;

namespace Racetimes.ReadModel
{
    public interface IAmReadModelForCompetitionAggregate :
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionCreatedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionRenamedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent>
    {
    }
}