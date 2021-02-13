using LatrunculiCore.Desk;
using System;
using System.Linq;

namespace Latrunculi
{
    public class DeskPrinter
    {
        private DeskManager desk;
        private DeskHistoryManager historyManager;

        public DeskPrinter(DeskManager desk, DeskHistoryManager historyManager)
        {
            this.desk = desk;
            this.historyManager = historyManager;
        }

        public void PrintDesk()
        {
            printLineSeparator();
            for (int y = desk.Size.Height - 1; y >= 0; y--)
            {
                Console.Write($"{y + 1} |");
                for (int x = 0; x < desk.Size.Width; x++)
                {
                    printCheckBox(new ChessBoxPosition(desk.Size, x, y));
                    Console.Write("|");
                }
                Console.WriteLine();
                printLineSeparator();
            }
            printLetters();
            Console.WriteLine();
        }

        private void printLineSeparator()
        {
            Console.WriteLine($"  {new String('-', desk.Size.Width * 4 + 1)}");
        }

        private void printLetters()
        {
            Console.Write("  ");
            foreach (var i in Enumerable.Range(0, desk.Size.Width))
            {
                Console.Write($"  {(char)('a' + i)} ");
            }
        }

        private void printCheckBox(ChessBoxPosition position)
        {
            var lastChange = historyManager.LastStep?.Changes.Find(change => change.Position.Index == position.Index);
            var state = desk.PlayingDesk[position.X, position.Y];
            ConsoleColor? color = null;
            ConsoleColor? backgroundColor = null;
            string symbol;
            if (lastChange != null)
            {
                if (lastChange.IsCaptured == true)
                {
                    backgroundColor = ConsoleColor.Red;
                }
                else if (lastChange.NewState == ChessBoxState.Empty)
                {
                    backgroundColor = ConsoleColor.DarkGray;
                }
                else if (lastChange.NewState == state)
                {
                    color = ConsoleColor.Green;
                }
            }
            switch (state)
            {
                case ChessBoxState.Black:
                    symbol = "b";
                    break;
                case ChessBoxState.White:
                    symbol = "w";
                    break;
                case ChessBoxState.Empty:
                default:
                    symbol = " ";
                    break;
            }
            Program.WriteColored($" {symbol} ", color, backgroundColor);
        }
    }
}
