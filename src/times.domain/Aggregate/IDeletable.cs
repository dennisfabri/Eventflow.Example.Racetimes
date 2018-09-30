namespace Racetimes.Domain.Aggregate
{
    interface IDeletable
    {
        bool IsDeleted { get; }
    }
}
