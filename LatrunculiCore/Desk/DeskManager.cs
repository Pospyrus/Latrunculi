using System;
using System.ComponentModel;

namespace LatrunculiCore.Desk
{
    public class DeskManager: INotifyPropertyChanged
    {
        public DeskSize Size { get; private set; }

        public ChessBoxState[,] PlayingDesk { get; private set; }

        public DeskManager Bind => this;

        public event EventHandler<ChangeSet> StepDone;
        public event EventHandler<ChangeSet> StepReverted;
        public event PropertyChangedEventHandler PropertyChanged;

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
            notifyPropertyChanged(nameof(Bind));
        }

        public void RevertStep(ChangeSet step)
        {
            foreach (var change in step.Changes)
            {
                PlayingDesk[change.Position.X, change.Position.Y] = change.OldState;
            }
            StepReverted?.Invoke(this, step);
            notifyPropertyChanged(nameof(Bind));
        }

        protected void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
