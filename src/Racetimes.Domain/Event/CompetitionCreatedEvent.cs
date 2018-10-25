using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
{
    [EventVersion("CompetitionCreated", 1)]
    public class CompetitionCreatedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public string Name { get; private set; }
        public string User { get; private set; }

        public CompetitionCreatedEvent(string user, string name)
        {
            User = user;
            Name = name;
        }
    }
}