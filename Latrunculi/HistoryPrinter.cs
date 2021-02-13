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
                string actual = (historyManager.HistoryIndex + 1) == i ? " <= aktuální" : "";
                Program.WriteColoredMulti(
                    ($"# Tah ", null, null),
                    ($"@{i++}", ConsoleColor.Yellow, null),
                    ($" - ", null, null),
                    ($"{step.Move}", ConsoleColor.Yellow, null),
                    (actual, ConsoleColor.Green, null)
                );
                Console.WriteLine();
                if (step.CapturedCount > 0)
                {
                    Program.WriteColoredMulti(
                        ($"Vyhozeno: ", null, null),
                        ($"{step.CapturedCount}", ConsoleColor.Yellow, null)
                    );
                    Console.WriteLine();
                }
                foreach (var change in step.Changes)
                {
                    Console.WriteLine($"# {Char.ToString((char)change.Position.Letter)}{change.Position.Number} původní: {change.OldState}, nová: {change.NewState}.");
                }
                Console.WriteLine("#####################################");
            }
        }
    }
}
