using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Racetimes.Domain.Extension
{
    class SnapshotNeverStrategy : ISnapshotStrategy
    {
        public static ISnapshotStrategy Default { get; } = new SnapshotNeverStrategy();

        private SnapshotNeverStrategy() { }

        public Task<bool> ShouldCreateSnapshotAsync(ISnapshotAggregateRoot snapshotAggregateRoot, CancellationToken cancellationToken)
        {
            return Task.FromResult<bool>(false);
        }
    }
}