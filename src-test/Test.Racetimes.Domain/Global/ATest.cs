using AutoFixture;
using AutoFixture.AutoMoq;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test.Racetimes.Domain.Global
{
    public abstract class ATest
    {
        protected IFixture Fixture { get; private set; }
        protected IDomainEventFactory DomainEventFactory;

        public ATest()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());

            Fixture.Customize<CompetitionId>(x => x.FromFactory(() => CompetitionId.New));
            Fixture.Customize<EntryId>(x => x.FromFactory(() => EntryId.New));
            Fixture.Customize<EventId>(c => c.FromFactory(() => EventId.New));

            DomainEventFactory = new DomainEventFactory();
        }

        protected T A<T>()
        {
            return Fixture.Create<T>();
        }

        protected IDomainEvent<CompetitionAggregate, CompetitionId> ToDomainEvent(
            CompetitionId competitionId,
            IAggregateEvent aggregateEvent,
            int aggregateSequenceNumber = 1)
        {
            var metadata = new Metadata
            {
                Timestamp = A<DateTimeOffset>(),
                SourceId = A<SourceId>(),
            };

            if (aggregateSequenceNumber == 0)
            {
                aggregateSequenceNumber = A<int>();
            }

            return DomainEventFactory.Create<CompetitionAggregate, CompetitionId>(
                aggregateEvent,
                metadata,
                competitionId,
                aggregateSequenceNumber);
        }

        protected IReadOnlyCollection<IDomainEvent> ToDomainEvents(CompetitionId competitionId, params IAggregateEvent[] events)
        {
            return ToDomainEvents(competitionId, 1, events);
        }

        protected IReadOnlyCollection<IDomainEvent> ToDomainEvents(CompetitionId competitionId, int sequenceNumber, params IAggregateEvent[] events)
        {
            return new ReadOnlyCollection<IDomainEvent>(events.Select((e, x) => ToDomainEvent(competitionId, e, sequenceNumber + x) as IDomainEvent).ToList());
        }
    }
}
