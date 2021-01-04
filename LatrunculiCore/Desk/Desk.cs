using System;
using System.Collections.Generic;

namespace LatrunculiCore.Desk
{
    public class DeskSize
    {
        public readonly int X;
        public readonly int Y;

        public DeskSize(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Desk
    {
        public readonly DeskSize Size;

        public readonly ChessBoxState[,] PlayingDesk;

        public event EventHandler<MoveStep> Moved;

        public Desk(DeskSize size)
        {
            Size = size;
            PlayingDesk = new ChessBoxState[Size.X, Size.Y];
        }

        public bool Move(ChessBoxReference from, ChessBoxReference to)
        {
            MoveStep step = new MoveStep();
            ChessBoxState stateFrom = PlayingDesk[from.X, from.Y];
            ChessBoxState stateTo = PlayingDesk[to.X, to.Y];
            step.AddChange(from, stateFrom, ChessBoxState.Empty);
            step.AddChange(to, stateTo, stateFrom);
            DoStep(step);
            Moved?.Invoke(this, step);           
            return true;
        }

        public void DoStep(MoveStep step)
        {
            foreach (var change in step.Changes)
                PlayingDesk[change.CheckBox.X, change.CheckBox.Y] = change.NewState;
        }

        public void RevertStep(MoveStep step)
        {
            foreach (var change in step.Changes)
                PlayingDesk[change.CheckBox.X, change.CheckBox.Y] = change.OldState;
        }
    }
}
