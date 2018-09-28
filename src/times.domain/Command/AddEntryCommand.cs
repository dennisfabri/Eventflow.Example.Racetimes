using times.domain.Aggregate;
using times.domain.Identity;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace times.domain.Command
{
    public class AddEntryCommand : Command<CompetitionAggregate, CompetitionId, IExecutionResult>
    {
        public EntryId EntryId { get; private set; }
        public string Name { get; private set; } = "";
        public string Discipline { get; private set; } = "";
        public int TimeInMillis { get; private set; } = 0;

        public AddEntryCommand(CompetitionId id, EntryId entryId, string discipline, string name, int timeInMillis) : base(id)
        {
            EntryId = entryId;
            Discipline = discipline;
            Name = name;
            TimeInMillis = timeInMillis;
        }
    }
}
