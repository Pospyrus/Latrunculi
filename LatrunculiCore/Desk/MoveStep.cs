using System.Collections.Generic;

namespace LatrunculiCore.Desk
{
    public class MoveStep
    {
        public readonly List<ChessBoxChange> Changes = new List<ChessBoxChange>();
     
        public void AddChange(ChessBoxReference checkBox, ChessBoxState oldState, ChessBoxState newState)
        {
            ChessBoxChange change = new ChessBoxChange
            {
                CheckBox = checkBox,
                OldState = oldState,
                NewState = newState
            };
            Changes.Add(change);
        }
    }
}
