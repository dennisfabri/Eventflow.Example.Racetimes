using System;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using System.Collections.Generic;
using System.Linq;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Specifications;
using EventFlow.Extensions;
using Racetimes.Domain.Aggregate.Extension;

namespace Racetimes.Domain.Aggregate
{
    [AggregateName("Competition")]
    public class CompetitionAggregate :
        SnapshotAggregateRoot<CompetitionAggregate, CompetitionId, CompetitionSnapshot>,
        IDeletableAggregateRoot,
        IEmit<CompetitionCreatedEvent>,
        IEmit<CompetitionRenamedEvent>,
        IEmit<EntryAddedEvent>,
        IEmit<EntryTimeChangedEvent>
    {
        public String Competitionname { get; private set; } = "";
        public String User { get; private set; } = "";
        public IList<EntryEntity> Entries { get; private set; } = new List<EntryEntity>();
        public bool IsDeleted { get; private set; } = false;

        private static ISpecification<IDeletableAggregateRoot> IsNewSpecification = new IsNewSpecification<IDeletableAggregateRoot>();
        private static ISpecification<IDeletableAggregateRoot> IsNotNewSpecification = new IsNotNewSpecification<IDeletableAggregateRoot>();
        private static ISpecification<IDeletableAggregateRoot> IsDeletableSpecification = IsNotNewSpecification.And(new IsNotDeletedSpecification<IDeletableAggregateRoot>());
        private static ISpecification<IDeletableAggregateRoot> IsRenamableSpecification = IsNotNewSpecification.And(new IsNotDeletedSpecification<IDeletableAggregateRoot>());
        private static ISpecification<string> IsNameEnteredSpecification = new IsNotNullOrEmptySpecification("name");
        private static ISpecification<string> IsUserEnteredSpecification = new IsNotNullOrEmptySpecification("user");

        private static ISpecification<string> IsEntryDisciplineEnteredSpecification = new IsNotNullOrEmptySpecification("discipline");
        private static ISpecification<string> IsEntryNameEnteredSpecification = new IsNotNullOrEmptySpecification("name");
        private static ISpecification<int> IsEntryTimeEnteredSpecification = new IsAtLeastSpecification(1, "time");


        public CompetitionAggregate(CompetitionId id, ISnapshotStrategy snapshotStrategy) : base(id, snapshotStrategy) { }

        #region Helpers

        private IExecutionResult ExecuteAfterValidation(Func<IAggregateEvent<CompetitionAggregate, CompetitionId>> evt, params Func<IExecutionResult>[] checks)
        {
            IExecutionResult ir = null;
            foreach (var check in checks)
            {
                ir = check();
                if (!ir.IsSuccess)
                {
                    return ir;
                }
            }
            IAggregateEvent<CompetitionAggregate, CompetitionId> @event = evt();
            if (@event != null)
            {
                Emit(@event);
                return ir ?? ExecutionResult.Success();
            }
            else
            {
                return ExecutionResult.Failed();
            }
        }

        #endregion

        #region Check and emit

        internal IExecutionResult Delete()
        {
            return ExecuteAfterValidation(
                () => new CompetitionDeletedEvent(Entries.Select(s => s.Id)),
                () => IsDeletableSpecification.IsNotSatisfiedByAsExecutionResult(this)
                );
        }

        public IExecutionResult Create(string user, string name)
        {
            return ExecuteAfterValidation(
                () =>
                {
                    user = user.Trim();
                    name = name.Trim();
                    return new CompetitionCreatedEvent(user, name);
                },
                () => IsNewSpecification.IsNotSatisfiedByAsExecutionResult(this),
                () => IsNameEnteredSpecification.IsNotSatisfiedByAsExecutionResult(name),
                () => IsUserEnteredSpecification.IsNotSatisfiedByAsExecutionResult(user)
            );
        }

        public IExecutionResult Rename(string name)
        {
            return ExecuteAfterValidation(
                () =>
                    {
                        name = name.Trim();
                        if (!Competitionname.Equals(name))
                        {
                            return new CompetitionRenamedEvent(name);
                        }
                        return null;
                    },
                () => IsRenamableSpecification.IsNotSatisfiedByAsExecutionResult(this),
                () => IsNameEnteredSpecification.IsNotSatisfiedByAsExecutionResult(name)
            );
        }

        internal IExecutionResult AddEntry(EntryId entryId, string discipline, string name, int timeInMillis)
        {
            return ExecuteAfterValidation(
                () => new EntryAddedEvent(entryId, discipline, name, timeInMillis),
                () => IsEntryDisciplineEnteredSpecification.IsNotSatisfiedByAsExecutionResult(discipline),
                () => IsEntryNameEnteredSpecification.IsNotSatisfiedByAsExecutionResult(name),
                () => IsEntryTimeEnteredSpecification.IsNotSatisfiedByAsExecutionResult(timeInMillis)
            );
        }

        internal IExecutionResult ChangeEntryTime(EntryId entryId, int timeInMillis)
        {
            return ExecuteAfterValidation(
                () => Entries.FirstOrDefault(e => e.Id == entryId)?.ChangeTime(timeInMillis)
                );
        }

        #endregion

        #region Apply

        public void Apply(CompetitionCreatedEvent aggregateEvent)
        {
            Competitionname = aggregateEvent.Name;
            User = aggregateEvent.User;
        }

        public void Apply(CompetitionRenamedEvent aggregateEvent)
        {
            Competitionname = aggregateEvent.Name;
        }

        public void Apply(CompetitionDeletedEvent aggregateEvent)
        {
            IsDeleted = true;
        }

        public void Apply(EntryAddedEvent aggregateEvent)
        {
            Entries.Add(new EntryEntity(aggregateEvent.EntryId, aggregateEvent.Discipline, aggregateEvent.Name, aggregateEvent.TimeInMillis));
        }

        public void Apply(EntryTimeChangedEvent aggregateEvent)
        {
            EntryEntity entry = Entries.First(e => e.Id == aggregateEvent.EntryId);
            entry.ChangeTime(aggregateEvent.TimeInMillis);
        }

        #endregion

        #region Snapshot

        private CompetitionSnapshot CreateSnapshot(CancellationToken cancellationToken)
        {
            return new CompetitionSnapshot(Competitionname, User, IsDeleted, Entries.Select(e => e.CreateSnapshot()).ToArray());
        }

        private void LoadSnapshot(CompetitionSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            Competitionname = snapshot.Competitionname;
            IsDeleted = snapshot.IsDeleted;
            User = snapshot.User;
            Entries = new List<EntryEntity>(snapshot.Entries.Select(e => new EntryEntity(e)));
        }

        protected override Task<CompetitionSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            // Note:
            // The following code causes the application to hang:
            // return new Task<CompetitionSnapshot>(() => CreateSnapshot(cancellationToken));
            // This version works correctly:
            return Task.FromResult(CreateSnapshot(cancellationToken));
        }

        protected override Task LoadSnapshotAsync(CompetitionSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            // Note: See comment in CreateSnapshotAsync
            // return new Task(() => { LoadSnapshot(snapshot, metadata, cancellationToken); return 0; });
            LoadSnapshot(snapshot, metadata, cancellationToken);
            return Task.FromResult(0);
        }

        #endregion
    }
}