using EventFlow.Specifications;
using System.Collections.Generic;

namespace Racetimes.Domain.Aggregate.Extension
{
    public class IsNotDeletedSpecification<TAggregateRoot> : Specification<TAggregateRoot>
        where TAggregateRoot : IDeletableAggregateRoot
    {
        protected override IEnumerable<string> IsNotSatisfiedBecause(TAggregateRoot i)
        {
            if (i.IsDeleted)
            {
                yield return "Aggreaget is deleted.";
            }
        }
    }
}
