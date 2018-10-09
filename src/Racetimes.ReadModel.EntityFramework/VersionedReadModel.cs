using EventFlow.ReadStores;
using System.ComponentModel.DataAnnotations;

namespace Racetimes.ReadModel.EntityFramework
{
    public class VersionedReadModel : IReadModel
    {
        public string Id { get; protected set; }

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
