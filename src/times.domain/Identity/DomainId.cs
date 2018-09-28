using EventFlow.Core;

namespace times.domain.Identity
{
    public class CompetitionId : Identity<CompetitionId>
    {
        public CompetitionId(string value) : base(value) { }
    }
}
