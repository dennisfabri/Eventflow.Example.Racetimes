using System;
using System.Collections.Generic;
using System.Text;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace Racetimes.Domain.Event
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