using EventFlow.Specifications;
using System.Collections.Generic;

namespace Racetimes.Domain.Aggregate.Extension
{
    public class IsAtLeastSpecification : Specification<int>
    {
        public string Name { get; }
        public int MinValue { get; }

        public IsAtLeastSpecification(int minValue, string name)
        {
            Name = name;
            MinValue = minValue;
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(int i)
        {
            if (i < MinValue)
            {
                yield return string.Format("{0} is lower than {1}.", Name, MinValue);
            }
        }
    }
}
