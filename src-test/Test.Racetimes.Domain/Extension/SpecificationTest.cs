using Xunit;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Aggregate.Extension;
using Racetimes.Domain.Identity;
using FluentAssertions;
using Racetimes.Domain.Event;
using System.Linq;
using Test.Racetimes.Domain.Global;
using EventFlow.Snapshots.Strategies;
using Test.Racetimes.Domain.Extension;

namespace Test.Racetimes.Domain
{
    public class SpecificationTest : ATest
    {
        #region IsNewSpecification

        [Fact]
        public void IsNewPositiveTest()
        {
            var spec = new IsNewSpecification<CompetitionAggregate>();
            var isNew = spec.IsSatisfiedBy(new CompetitionAggregate(CompetitionId.New, SnapshotEveryFewVersionsStrategy.Default));
            isNew.Should().BeTrue();
        }

        [Fact]
        public void IsNewNegativeTest()
        {
            var spec = new IsNewSpecification<CompetitionAggregate>();
            var competition = new CompetitionAggregate(CompetitionId.New, SnapshotNeverStrategy.Default);
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
            var isDeleted = spec.IsSatisfiedBy(new CompetitionAggregate(CompetitionId.New, SnapshotNeverStrategy.Default));
            isDeleted.Should().BeTrue();
        }

        [Fact]
        public void IsNotDeletedNegativeTest()
        {
            var spec = new IsNotDeletedSpecification<CompetitionAggregate>();
            var competition = new CompetitionAggregate(CompetitionId.New, SnapshotNeverStrategy.Default);
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
            var competition = new CompetitionAggregate(CompetitionId.New, SnapshotNeverStrategy.Default);
            competition.ApplyEvents(ToDomainEvents(competition.Id, new CompetitionCreatedEvent("user", "name")));
            var isNotNew = spec.IsSatisfiedBy(competition);
            isNotNew.Should().BeTrue();
        }

        [Fact]
        public void IsNotNewNegativeTest()
        {
            var spec = new IsNotNewSpecification<CompetitionAggregate>();
            var isNotNew = spec.IsSatisfiedBy(new CompetitionAggregate(CompetitionId.New, SnapshotNeverStrategy.Default));
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
    }
}
