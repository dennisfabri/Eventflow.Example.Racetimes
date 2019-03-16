using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
{
    [EventVersion("CompetitionRegistered", 1)]
    public class CompetitionRegisteredEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {
        public string Name { get; private set; }
        public string User { get; private set; }

        public CompetitionRegisteredEvent(string user, string name)
        {
            User = user;
            Name = name;
        }
    }
}