using System;
using System.Collections.Generic;

namespace LatrunculiCore.Desk
{
    public class DeskHistoryManager
    {
        public event EventHandler<ChangeSet> GoingPrev;
        public event EventHandler<ChangeSet> GoingBack;

        public int ActualRound { get; private set; } = -1;
        public List<ChangeSet> Steps { get; private set; } = new List<ChangeSet>();
        public ChessBoxState ActualPlayer => ActualRound % 2 == 0 ? ChessBoxState.Black : ChessBoxState.White;

        public void Add(ChangeSet step)
        {
            Steps.RemoveRange(ActualRound + 1, Steps.Count - ActualRound - 1);
            Steps.Add(step);
            ActualRound++;
        }

        public void Back()
        {
            if (ActualRound > -1)
                goHistoryBackTo(ActualRound - 1);
        }

        public void Prev()
        {
            if (ActualRound < Steps.Count - 1)
                goHistoryForwardTo(ActualRound + 1);
        }

        public void GoTo(int newIndex)
        {
            if (newIndex < 0 || newIndex > Steps.Count - 1)
                throw new ArgumentException($"Číslo tahu musí být v rozmezí 0 až {Steps.Count}.");
            if (newIndex > ActualRound)
                goHistoryForwardTo(newIndex);
            else
                goHistoryBackTo(newIndex);
        }

        private void goHistoryBackTo(int newIndex)
        {
            while (ActualRound > newIndex)
            {
                GoingBack?.Invoke(this, Steps[ActualRound]);
                ActualRound--;
            }
        }

        private void goHistoryForwardTo(int newIndex)
        {
            while (ActualRound < newIndex)
            {
                GoingPrev?.Invoke(this, Steps[ActualRound + 1]);
                ActualRound++;
            }
        }
    }
}
