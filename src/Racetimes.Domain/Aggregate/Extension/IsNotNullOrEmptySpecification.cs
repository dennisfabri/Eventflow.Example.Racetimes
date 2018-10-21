using EventFlow.Specifications;
using System.Collections.Generic;

namespace Racetimes.Domain.Aggregate.Extension
{
    public class IsNotNullOrEmptySpecification : Specification<string>
    {
        public string Name { get; private set; }

        public IsNotNullOrEmptySpecification(string name)
        {
            Name = name;
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(string i)
        {
            if (string.IsNullOrEmpty(i))
            {
                yield return string.Format("{0} is null or empty.", Name);
            }
        }
    }
}
