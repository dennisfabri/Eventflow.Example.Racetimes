using EventFlow.ReadStores;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;

namespace Racetimes.ReadModel
{
    public interface IAmReadModelForEntryEntity :
        IAmReadModelFor<CompetitionAggregate, CompetitionId, EntryAddedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, EntryTimeChangedEvent>,
        IAmReadModelFor<CompetitionAggregate, CompetitionId, CompetitionDeletedEvent>
    {
    }
}