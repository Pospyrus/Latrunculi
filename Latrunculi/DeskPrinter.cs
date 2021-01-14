using LatrunculiCore.Desk;
using System;
using System.Linq;

namespace Latrunculi
{
    public class DeskPrinter
    {
        private DeskManager desk;

        public DeskPrinter(DeskManager desk)
        {
            this.desk = desk;
        }

        public void PrintDesk()
        {
            printLineSeparator();
            for (int y = desk.Size.Height - 1; y >= 0; y--)
            {
                Console.Write($"{y + 1} |");
                for (int x = 0; x < desk.Size.Width; x++)
                {
                    printCheckBox(desk.PlayingDesk[x, y]);
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
            Console.Write("  ");
            for (int x = desk.Size.Width * 4 + 1; x > 0; x--)
                Console.Write("-");
            Console.WriteLine();
        }

        private void printLetters()
        {
            Console.Write("  ");
            foreach (var i in Enumerable.Range(0, desk.Size.Width))
            {
                Console.Write($"  {(char)('a' + i)} ");
            }
        }

        private void printCheckBox(ChessBoxState state)
        {
            string symbol;
            switch (state)
            {
                case ChessBoxState.Black:
                    symbol = "k";
                    break;
                case ChessBoxState.White:
                    symbol = "w";
                    break;
                case ChessBoxState.Empty:
                default:
                    symbol = " ";
                    break;
            }
            Console.Write($" {symbol} ");
        }
    }
}
