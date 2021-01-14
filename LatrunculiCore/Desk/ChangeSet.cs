using System.Collections.Generic;

namespace LatrunculiCore.Desk
{
    public class ChangeSet
    {
        public readonly List<ChessBoxChange> Changes = new List<ChessBoxChange>();

        public ChangeSet()
        {
        }

        public ChangeSet(IEnumerable<ChessBoxChange> changes)
        {
            Changes.AddRange(changes);
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
