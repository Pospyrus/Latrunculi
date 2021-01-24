using LatrunculiCore.Desk;
using System;

namespace Latrunculi
{
    public class HistoryPrinter
    {
        private DeskHistoryManager historyManager;

        public HistoryPrinter(DeskHistoryManager historyManager)
        {
            this.historyManager = historyManager;
        }

        public void PrintHistory()
        {
            Console.WriteLine("#####################################");
            Console.WriteLine("# History");
            Console.WriteLine("#####################################");
            int i = 1;
            foreach (var step in historyManager.Steps)
            {
                string actual = (historyManager.ActualRound + 1) == i ? " <= aktuální" : "";
                Console.WriteLine($"# Tah @{i++}{actual}");
                foreach (var change in step.Changes)
                    Console.WriteLine($"# {Char.ToString((char)change.Position.Letter)}{change.Position.Number} původní: {change.OldState}, nová: {change.NewState}.");
                Console.WriteLine("#####################################");
            }
        }
    }
}
