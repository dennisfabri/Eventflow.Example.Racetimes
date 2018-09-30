using EventFlow.Core;

namespace Racetimes.Domain.Identity
{
    public class CompetitionId : Identity<CompetitionId>
    {
        public CompetitionId(string value) : base(value) { }
    }
}
