using System;

namespace LatrunculiCore.Desk
{
    public class DeskManager
    {
        public DeskSize Size { get; private set; }

        public ChessBoxState[,] PlayingDesk { get; private set; }

        public DeskManager Bind => this;

        public event EventHandler<ChangeSet> StepDone;
        public event EventHandler<ChangeSet> StepReverted;
        public event EventHandler DeskChanged;

        public DeskManager(DeskSize size)
        {
            Size = size;
            PlayingDesk = new ChessBoxState[Size.Width, Size.Height];
        }

        public ChessBoxState GetState(ChessBoxPosition position)
        {
            return PlayingDesk[position.X, position.Y];
        }

        public void DoStep(ChangeSet step)
        {
            foreach (var change in step.Changes)
            {
                PlayingDesk[change.Position.X, change.Position.Y] = change.NewState;
            }
            StepDone?.Invoke(this, step);
            DeskChanged?.Invoke(this, null);
        }

        public void RevertStep(ChangeSet step)
        {
            foreach (var change in step.Changes)
            {
                PlayingDesk[change.Position.X, change.Position.Y] = change.OldState;
            }
            StepReverted?.Invoke(this, step);
            DeskChanged?.Invoke(this, null);
        }

        public void Dispose()
        {
            PlayingDesk = null;
            Size = null;
        }
    }
}
