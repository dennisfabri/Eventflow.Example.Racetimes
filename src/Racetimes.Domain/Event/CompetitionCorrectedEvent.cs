using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
{
    [EventVersion("CompetitonCorrected", 1)]
    public class CompetitionCorrectedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public string Name { get; private set; }

        public CompetitionCorrectedEvent(string name)
        {
            Name = name;
        }
    }
}