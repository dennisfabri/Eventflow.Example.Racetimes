using Xunit;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Aggregate.Extension;
using Racetimes.Domain.Identity;
using FluentAssertions;
using Racetimes.Domain.Event;
using System.Collections.ObjectModel;
using EventFlow.Aggregates;
using EventFlow.EventStores;
using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using EventFlow.Core;

namespace Test.Racetimes.Domain
{
    public class SpecificationTest
    {
        [Fact]
        public void IsNewPositiveTest()
        {
            var spec = new IsNewSpecification<CompetitionAggregate>();
            var isNew = spec.IsSatisfiedBy(new CompetitionAggregate(CompetitionId.New));
            isNew.Should().BeTrue();
        }

        [Fact]
        public void IsNewNegativeTest()
        {
            var spec = new IsNewSpecification<CompetitionAggregate>();
            var competition = new CompetitionAggregate(CompetitionId.New);
            competition.ApplyEvents(new ReadOnlyCollection<IDomainEvent>(new IDomainEvent[] { ToDomainEvent(competition.Id, new CompetitionCreatedEvent("user", "name"), 1) }));
            var isNew = spec.IsSatisfiedBy(competition);
            isNew.Should().BeFalse();
        }

        #region Tools

        protected IFixture Fixture { get; private set; }
        protected IDomainEventFactory DomainEventFactory;

        public SpecificationTest()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());

            Fixture.Customize<CompetitionId>(x => x.FromFactory(() => CompetitionId.New));
            Fixture.Customize<EntryId>(x => x.FromFactory(() => EntryId.New));
            Fixture.Customize<EventId>(c => c.FromFactory(() => EventId.New));
            // Fixture.Customize<Label>(s => s.FromFactory(() => Label.Named($"label-{Guid.NewGuid():D}")));

            DomainEventFactory = new DomainEventFactory();
        }

        protected T A<T>()
        {
            return Fixture.Create<T>();
        }

        protected IDomainEvent<CompetitionAggregate, CompetitionId> ToDomainEvent<TAggregateEvent>(
            CompetitionId competitionId,
            TAggregateEvent aggregateEvent,
            int aggregateSequenceNumber = 0)
            where TAggregateEvent : IAggregateEvent
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
        #endregion
    }
}
