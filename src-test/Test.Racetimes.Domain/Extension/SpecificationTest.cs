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
using System.Linq;
using System.Collections.Generic;

namespace Test.Racetimes.Domain
{
    public class SpecificationTest
    {
        #region IsNewSpecification

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
            competition.ApplyEvents(ToDomainEvents(competition.Id, new CompetitionCreatedEvent("user", "name")));
            var isNew = spec.IsSatisfiedBy(competition);
            isNew.Should().BeFalse();
        }

        #endregion

        #region IsNotDeletedSpecification

        [Fact]
        public void IsNotDeletedPositiveTest()
        {
            var spec = new IsNotDeletedSpecification<CompetitionAggregate>();
            var isDeleted = spec.IsSatisfiedBy(new CompetitionAggregate(CompetitionId.New));
            isDeleted.Should().BeTrue();
        }

        [Fact]
        public void IsNotDeletedNegativeTest()
        {
            var spec = new IsNotDeletedSpecification<CompetitionAggregate>();
            var competition = new CompetitionAggregate(CompetitionId.New);
            competition.ApplyEvents(ToDomainEvents(competition.Id, new CompetitionCreatedEvent("user", "name"), new CompetitionDeletedEvent(new EntryId[0].AsEnumerable())));
            var isNew = spec.IsSatisfiedBy(competition);
            isNew.Should().BeFalse();
        }

        #endregion

        #region IsNotNewSpecification

        [Fact]
        public void IsNotNewPositiveTest()
        {
            var spec = new IsNotNewSpecification<CompetitionAggregate>();
            var competition = new CompetitionAggregate(CompetitionId.New);
            competition.ApplyEvents(ToDomainEvents(competition.Id, new CompetitionCreatedEvent("user", "name")));
            var isNotNew = spec.IsSatisfiedBy(competition);
            isNotNew.Should().BeTrue();
        }

        [Fact]
        public void IsNotNewNegativeTest()
        {
            var spec = new IsNotNewSpecification<CompetitionAggregate>();
            var isNotNew = spec.IsSatisfiedBy(new CompetitionAggregate(CompetitionId.New));
            isNotNew.Should().BeFalse();
        }

        #endregion

        #region IsNullOrEmptySpecification

        [Fact]
        public void IsNotNullOrEmptyPositiveTest()
        {
            var spec = new IsNotNullOrEmptySpecification("positive");
            var isNotNullOrEmpty = spec.IsSatisfiedBy("test");
            isNotNullOrEmpty.Should().BeTrue();
        }

        [Fact]
        public void IsNotNullOrEmptyNegativeTest1()
        {
            var spec = new IsNotNullOrEmptySpecification("negative1");
            var isNotNullOrEmpty1 = spec.IsSatisfiedBy(null);
            isNotNullOrEmpty1.Should().BeFalse();
        }

        [Fact]
        public void IsNotNullOrEmptyNegativeTest2()
        {
            var spec = new IsNotNullOrEmptySpecification("negative2");
            var isNotNullOrEmpty2 = spec.IsSatisfiedBy("");
            isNotNullOrEmpty2.Should().BeFalse();
        }

        #endregion


        #region Tools

        protected IFixture Fixture { get; private set; }
        protected IDomainEventFactory DomainEventFactory;

        public SpecificationTest()
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
        #endregion
    }
}
