using EventFlow.Aggregates;
using EventFlow.Specifications;
using System.Collections.Generic;

namespace Racetimes.Domain.Aggregate.Extension
{
    public class IsNotNewSpecification<TAggregateRoot> : Specification<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {

        private static readonly string[] Errors = new string[] { "Aggregate is new." };
        private static readonly string[] NoErrors = new string[] { };

        protected override IEnumerable<string> IsNotSatisfiedBecause(TAggregateRoot i)
        {
            return i.IsNew ? Errors : NoErrors;
        }
    }
}
