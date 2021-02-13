using LatrunculiCore.Desk;
using System;
using System.Linq;
using System.Text;

namespace Latrunculi
{
    public class DeskToString
    {
        private DeskManager desk;
        private DeskHistoryManager historyManager;

        public DeskToString(DeskManager desk, DeskHistoryManager historyManager)
        {
            this.desk = desk;
            this.historyManager = historyManager;
        }

        public string GetDeskAsString()
        {
            StringBuilder sb = new StringBuilder(2 * desk.Size.Width * desk.Size.Height);
            sb.Append(getLineSeparator());
            for (int y = desk.Size.Height - 1; y >= 0; y--)
            {
                string inner = string.Join(" | ",
                    Enumerable.Range(0, desk.Size.Width)
                        .Select(x => getCheckBox(new ChessBoxPosition(desk.Size, x, y)))
                );
                sb.Append($"{y + 1} | {inner} |\n");
                sb.Append(getLineSeparator());
            }
            sb.Append(getLetters());
            return sb.ToString();
        }

        private string getLineSeparator()
        {
            return $"  {new String('-', desk.Size.Width * 4 + 1)}\n";
        }

        private string getLetters()
        {
            return $"  {string.Concat(Enumerable.Range(0, desk.Size.Width).Select(i => $"  {(char)('a' + i)} "))}\n";
        }

        private string getCheckBox(ChessBoxPosition position)
        {
            var lastChange = historyManager.LastStep?.Changes.Find(change => change.Position.Index == position.Index);
            var state = desk.PlayingDesk[position.X, position.Y];
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
