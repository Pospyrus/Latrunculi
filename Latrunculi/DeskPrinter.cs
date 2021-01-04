using LatrunculiCore.Desk;
using System;
using System.Linq;

namespace Latrunculi
{
    public class DeskPrinter
    {
        private Desk desk;

        public DeskPrinter(Desk desk)
        {
            this.desk = desk;
        }

        public void PrintDesk()
        {
            printLineSeparator();
            for (int y = desk.Size.Y - 1; y >= 0; y--)
            {
                Console.Write($"{y + 1} |");
                for (int x = 0; x < desk.Size.X; x++)
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
            for (int x = desk.Size.X * 4 + 1; x > 0; x--)
                Console.Write("-");
            Console.WriteLine();
        }

        private void printLetters()
        {
            Console.Write("  ");
            foreach (var i in Enumerable.Range(0, desk.Size.X))
            {
                Console.Write($"  {(char)('a' + i)} ");
            }
        }

        private void printCheckBox(ChessBoxState state)
        {
            string symbol;
            switch (state)
            {
                case ChessBoxState.BlackDame:
                    symbol = "K";
                    break;
                case ChessBoxState.BlackPawn:
                    symbol = "k";
                    break;
                case ChessBoxState.WhiteDame:
                    symbol = "W";
                    break;
                case ChessBoxState.WhitePawn:
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
