using EventFlow.Core;

namespace Racetimes.Domain.Identity
{
    public class EntryId : Identity<EntryId>
    {
        public EntryId(string value) : base(value) { }
    }
}
