using System;
using System.Collections.Generic;

namespace LatrunculiCore.Desk
{
    public class DeskHistoryManager
    {
        public event EventHandler<ChangeSet> GoingPrev;
        public event EventHandler<ChangeSet> GoingBack;

        public int HistoryIndex { get; private set; } = -1;
        public int ActualRound => HistoryIndex + 2;
        public List<ChangeSet> Steps { get; private set; } = new List<ChangeSet>();
        public ChangeSet LastStep => HistoryIndex >= 0 ? Steps[HistoryIndex] : null;
        public ChessBoxState ActualPlayer => HistoryIndex % 2 == 0 ? ChessBoxState.Black : ChessBoxState.White;
        public ChessBoxState EnemyPlayer => ActualPlayer == ChessBoxState.White ? ChessBoxState.Black : ChessBoxState.White;

        public void Add(ChangeSet step)
        {
            Steps.RemoveRange(HistoryIndex + 1, Steps.Count - HistoryIndex - 1);
            Steps.Add(step);
            HistoryIndex++;
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

        public void GoTo(int newIndex)
        {
            if (newIndex < -1 || newIndex > Steps.Count - 1)
                throw new ArgumentException($"Číslo tahu musí být v rozmezí 0 až {Steps.Count}.");
            if (newIndex > HistoryIndex)
                goHistoryForwardTo(newIndex);
            else
                goHistoryBackTo(newIndex);
        }

        public void LoadHistory(List<ChangeSet> steps)
        {
            GoTo(-1);
            Steps = steps;
            GoTo(Steps.Count + -1);
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
    }
}
