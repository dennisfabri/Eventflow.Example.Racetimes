using EventFlow.ReadStores;
using System;
using System.Collections.Generic;
using System.Text;

namespace times.domain.Readmodel
{
    public class VersionedReadModel : IReadModel
    {
        public string AggregateId { get; protected set; }
        public int Version { get; private set; } = 0;

        protected void Update()
        {
            Version++;
        }
    }
}
