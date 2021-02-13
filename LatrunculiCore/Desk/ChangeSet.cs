using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LatrunculiCore.Desk
{
    public class ChangeSet
    {
        [JsonInclude]
        public readonly List<ChessBoxChange> Changes;

        [JsonInclude]
        public readonly Move Move;

        [JsonIgnore]
        public int CapturedCount => Changes.Count(change => change.IsCaptured);

        public ChangeSet(Move move): this(move, null)
        {
        }

        [JsonConstructor]
        public ChangeSet(Move move, List<ChessBoxChange> changes)
        {
            Move = move;
            Changes = changes?.ToList() ?? new List<ChessBoxChange>();
        }

        public void AddChange(ChessBoxPosition checkBox, ChessBoxState oldState, ChessBoxState newState)
        {
            ChessBoxChange change = new ChessBoxChange
            {
                Position = checkBox,
                OldState = oldState,
                NewState = newState
            };
            Changes.Add(change);
        }

        public void AddChange(ChessBoxChange change)
        {
            Changes.Add(change);
        }

        public void AddChangesRange(IEnumerable<ChessBoxChange> changes)
        {
            Changes.AddRange(changes);
        }
    }
}
