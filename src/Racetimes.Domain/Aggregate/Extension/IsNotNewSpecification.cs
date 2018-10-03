using EventFlow.Aggregates;
using EventFlow.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Racetimes.Domain.Aggregate.Extension
{
    public class IsNotNewSpecification<TAggregateRoot> : Specification<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {
        protected override IEnumerable<string> IsNotSatisfiedBecause(TAggregateRoot i)
        {
            if (i.IsNew)
            {
                yield return "Aggregate is new.";
            }
        }
    }
}
