using System;
using System.Collections.Generic;
using System.Text;
using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace times.domain.Event
{
    [EventVersion("CompetitonRenamed", 1)]
    public class CompetitionRenamedEvent : IAggregateEvent<CompetitionAggregate, CompetitionId>
    {

        public string Name { get; private set; }

        public CompetitionRenamedEvent(string name)
        {
            Name = name;
        }
    }
}
