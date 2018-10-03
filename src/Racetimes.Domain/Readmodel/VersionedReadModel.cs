using EventFlow.MsSql.ReadStores.Attributes;
using EventFlow.ReadStores;

namespace Racetimes.Domain.Readmodel
{
    public class VersionedReadModel : IReadModel
    {
        public string AggregateId { get; protected set; }

        [MsSqlReadModelVersionColumn]
        public int Version { get; set; }
    }
}
