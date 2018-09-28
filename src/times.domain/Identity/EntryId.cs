using EventFlow.Core;

namespace times.domain.Identity
{
    public class EntryId : Identity<EntryId>
    {
        public EntryId(string value) : base(value) { }
    }
}
