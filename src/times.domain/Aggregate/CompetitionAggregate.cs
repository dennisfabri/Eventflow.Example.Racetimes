using System;
using times.domain.Event;
using times.domain.Identity;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using System.Collections.Generic;
using System.Linq;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using System.Threading;
using System.Threading.Tasks;

namespace times.domain.Aggregate
{
    [AggregateName("Competition")]
    public class CompetitionAggregate :
        SnapshotAggregateRoot<CompetitionAggregate, CompetitionId, CompetitionSnapshot>,
        IDeletable,
        IEmit<CompetitionCreatedEvent>,
        IEmit<CompetitionRenamedEvent>,
        IEmit<EntryAddedEvent>,
        IEmit<EntryTimeChangedEvent>
    {
        public String Competitionname { get; private set; } = "";
        public String User { get; private set; } = "";
        public IList<EntryEntity> Entries { get; private set; } = new List<EntryEntity>();
        public bool IsDeleted { get; private set; } = false;

        internal IExecutionResult Delete()
        {
            if (!Exists())
            {
                return ExecutionResult.Failed("competition must exist.");
            }
            Emit(new CompetitionDeletedEvent(Entries.Select(s => s.Id)));
            return ExecutionResult.Success();
        }

        private bool Exists()
        {
            return !IsNew && !IsDeleted;
        }

        public CompetitionAggregate(CompetitionId id) : base(id, SnapshotEveryFewVersionsStrategy.Default) { }

        public IExecutionResult Create(string user, string name)
        {
            if (!this.IsNew)
            {
                return ExecutionResult.Failed("Aggregate must be new.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return ExecutionResult.Failed("name must not be null or emtpy.");
            }
            if (string.IsNullOrWhiteSpace(user))
            {
                return ExecutionResult.Failed("user must not be null or emtpy.");
            }

            user = user.Trim();
            name = name.Trim();
            if (Competitionname.Equals(name) && User.Equals(user))
            {
                return ExecutionResult.Success();
            }
            Emit(new CompetitionCreatedEvent(user, name));

            return ExecutionResult.Success();
        }

        public IExecutionResult Rename(string name)
        {
            if (Exists())
            {
                return ExecutionResult.Failed("competition must exist.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return ExecutionResult.Failed("name must not be null or emtpy.");
            }
            name = name.Trim();
            if (Competitionname.Equals(name))
            {
                return ExecutionResult.Success();
            }
            Emit(new CompetitionRenamedEvent(name));

            return ExecutionResult.Success();
        }

        internal IExecutionResult AddEntry(EntryId entryId, string discipline, string name, int timeInMillis)
        {
            Emit(new EntryAddedEvent(entryId, discipline, name, timeInMillis));
            return ExecutionResult.Success();
        }

        internal IExecutionResult ChangeEntryTime(EntryId entryId, int timeInMillis)
        {
            Emit(new EntryTimeChangedEvent(entryId, timeInMillis));
            return ExecutionResult.Success();
        }

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

        private CompetitionSnapshot CreateSnapshot(CancellationToken cancellationToken)
        {
            CompetitionSnapshot cs = new CompetitionSnapshot()
            {
                Competitionname = Competitionname,
                IsDeleted = IsDeleted,
                User = User,
                Entries = Entries.Select(e => e.CreateSnapshot()).ToArray()
            };
            return cs;
        }

        protected void LoadSnapshot(CompetitionSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
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
    }
}