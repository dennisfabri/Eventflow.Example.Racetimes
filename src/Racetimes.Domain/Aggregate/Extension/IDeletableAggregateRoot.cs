using EventFlow.Aggregates;

namespace Racetimes.Domain.Aggregate.Extension
{
    public interface IDeletableAggregateRoot : IAggregateRoot
    {
        bool IsDeleted { get; }
    }
}
