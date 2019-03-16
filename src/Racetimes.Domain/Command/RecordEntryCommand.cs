using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace Racetimes.Domain.Command
{
    public class RecordEntryCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public EntryId EntryId { get; private set; }
        public string Name { get; private set; } = "";
        public string Discipline { get; private set; } = "";
        public int TimeInMillis { get; private set; } = 0;

        public RecordEntryCommand(CompetitionId id, EntryId entryId, string discipline, string name, int timeInMillis) : base(id)
        {
            EntryId = entryId;
            Discipline = discipline;
            Name = name;
            TimeInMillis = timeInMillis;
        }
    }
}
