using System;
using System.Collections.Generic;

namespace LatrunculiCore.Desk
{
    public class DeskHistoryManager
    {
        private Desk desk;       

        public int Index { get; private set; } = -1;
        public List<MoveStep> Steps { get; private set; } = new List<MoveStep>();

        public DeskHistoryManager(Desk desk)
        {
            this.desk = desk;
        }

        public void Add(MoveStep step)
        {
            Steps.RemoveRange(Index + 1, Steps.Count - Index - 1);
            Steps.Add(step);
            Index++;
        }

        public void Back()
        {
            if (Index > -1)
                goHistoryBackTo(Index - 1);
        }

        public void Prev()
        {
            if (Index < Steps.Count - 1)
                goHistoryForwardTo(Index + 1);
        }

        public void GoTo(int newIndex)
        {
            if (newIndex < 0 || newIndex > Steps.Count - 1)
                throw new ArgumentException($"Číslo tahu musí být v rozmezí 0 až {Steps.Count}.");
            if (newIndex > Index)
                goHistoryForwardTo(newIndex);
            else
                goHistoryBackTo(newIndex);
        }

        private void goHistoryBackTo(int newIndex)
        {
            while (Index > newIndex)
            {
                desk.RevertStep(Steps[Index]);
                Index--;
            }
        }

        private void goHistoryForwardTo(int newIndex)
        {
            while (Index < newIndex)
            {
                desk.DoStep(Steps[Index + 1]);
                Index++;
            }
        }
    }
}
