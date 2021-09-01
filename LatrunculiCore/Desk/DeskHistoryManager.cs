using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LatrunculiCore.Desk
{
    public class DeskHistoryManager : INotifyPropertyChanged
    {
        public event EventHandler<ChangeSet> GoingPrev;
        public event EventHandler<ChangeSet> GoingBack;
        public event PropertyChangedEventHandler PropertyChanged;

        private int historyIndex = -1;

        public int HistoryIndex { 
            get => historyIndex; 
            private set {
                if (value != historyIndex)
                {
                    historyIndex = value;
                    notifyPropertyChanged(nameof(HistoryIndex));
                }
            }
        }

        public int ActualRound => HistoryIndex + 2;
        public ObservableCollection<ChangeSet> Steps { get; private set; } = new ObservableCollection<ChangeSet>();
        public ChangeSet LastStep => HistoryIndex >= 0 ? Steps[HistoryIndex] : null;
        public ChessBoxState StartingPlayer => ChessBoxState.White;
        public ChessBoxState ActualPlayer => HistoryIndex % 2 == 0 ? ChessBoxState.Black : ChessBoxState.White;
        public ChessBoxState EnemyPlayer => ActualPlayer == ChessBoxState.White ? ChessBoxState.Black : ChessBoxState.White;
        public bool IsLastIndexSelected => Steps.Count == HistoryIndex + 1;

        public void Add(ChangeSet step)
        {
            while (Steps.Count > HistoryIndex + 1)
            {
                Steps.RemoveAt(Steps.Count - 1);
            }
            Steps.Add(step);
            HistoryIndex++;
            notifyPropertyChanged(nameof(Steps));
        }

        public void Back()
        {
            if (HistoryIndex > -1)
                goHistoryBackTo(HistoryIndex - 1);
        }

        public void Prev()
        {
            if (HistoryIndex < Steps.Count - 1)
                goHistoryForwardTo(HistoryIndex + 1);
        }

        public void NewGame()
        {
            goHistoryBackTo(-1);
            Steps.Clear();
            notifyPropertyChanged(nameof(Steps));
        }

        public void GoTo(int newIndex)
        {
            if (newIndex < -1 || newIndex > Steps.Count - 1)
                throw new ArgumentException($"Číslo tahu musí být v rozmezí 0 až {Steps.Count}.");
            if (newIndex > HistoryIndex)
                goHistoryForwardTo(newIndex);
            else
                goHistoryBackTo(newIndex);
        }

        public void LoadHistory(IEnumerable<ChangeSet> steps)
        {
            GoTo(-1);
            Steps = new ObservableCollection<ChangeSet>(steps);
            GoTo(Steps.Count + -1);
            notifyPropertyChanged(nameof(Steps));
        }

        private void goHistoryBackTo(int newIndex)
        {
            while (HistoryIndex > newIndex)
            {
                GoingBack?.Invoke(this, Steps[HistoryIndex]);
                HistoryIndex--;
            }
        }

        private void goHistoryForwardTo(int newIndex)
        {
            while (HistoryIndex < newIndex)
            {
                GoingPrev?.Invoke(this, Steps[HistoryIndex + 1]);
                HistoryIndex++;
            }
        }

        private void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
