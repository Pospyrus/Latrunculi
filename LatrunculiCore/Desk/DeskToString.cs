using LatrunculiCore;
using LatrunculiCore.Desk;
using System;
using System.Linq;
using System.Text;

namespace Latrunculi
{
    public class DeskToString
    {
        public string GetDeskAsString(LatrunculiApp latrunculi)
        {
            var desk = latrunculi.Desk;
            StringBuilder sb = new StringBuilder(2 * desk.Size.Width * desk.Size.Height);
            sb.Append(getLineSeparator(desk));
            for (int y = desk.Size.Height - 1; y >= 0; y--)
            {
                string inner = string.Join(" | ",
                    Enumerable.Range(0, desk.Size.Width)
                        .Select(x => getCheckBox(latrunculi, new ChessBoxPosition(desk.Size, x, y)))
                );
                sb.Append($"{y + 1} | {inner} |\n");
                sb.Append(getLineSeparator(desk));
            }
            sb.Append(getLetters(desk));
            return sb.ToString();
        }

        private string getLineSeparator(DeskManager desk)
        {
            return $"  {new String('-', desk.Size.Width * 4 + 1)}\n";
        }

        private string getLetters(DeskManager desk)
        {
            return $"  {string.Concat(Enumerable.Range(0, desk.Size.Width).Select(i => $"  {(char)('a' + i)} "))}\n";
        }

        private string getCheckBox(LatrunculiApp latrunculi, ChessBoxPosition position)
        {
            var lastChange = latrunculi.HistoryManager.LastStep?.Changes.Find(change => change.Position.Index == position.Index);
            var state = latrunculi.Desk.PlayingDesk[position.X, position.Y];
            string symbol;
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
            if (lastChange != null)
            {
                if (lastChange.IsCaptured == true)
                {
                    symbol = "X";
                }
                else if (lastChange.NewState == ChessBoxState.Empty)
                {
                    symbol = "_";
                }
                else if (lastChange.NewState == state)
                {
                    symbol = symbol.ToUpper();
                }
            }
            return symbol;
        }
    }
}
