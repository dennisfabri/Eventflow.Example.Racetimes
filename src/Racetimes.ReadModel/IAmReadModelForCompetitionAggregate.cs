using EventFlow.ReadStores;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;

namespace Racetimes.ReadModel
{
    public interface IAmReadModelForCompetitionAggregate :
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionRegisteredEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionCorrectedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent>
    {
    }
}