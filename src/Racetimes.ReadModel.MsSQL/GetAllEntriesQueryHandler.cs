using EventFlow.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.ReadModel.MsSql
{
    public class GetAllEntriesQueryHandler : IQueryHandler<GetAllEntriesQuery, EntryReadModel[]>
    {
        public Task<EntryReadModel[]> ExecuteQueryAsync(GetAllEntriesQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new EntryReadModel[0]);
        }
    }
}