using Latrunculi;
using LatrunculiCore.Desk;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace LatrunculiCore.Players
{
    public class ConsolePlayer : IPlayer
    {
        private HistoryPrinter historyPrinter;
        private DeskHistoryManager historyManager;
        private DeskSize deskSize;
        private CommandManager commandManager;

        public ConsolePlayer(HistoryPrinter historyPrinter, DeskHistoryManager historyManager, DeskSize deskSize, CommandManager commandManager)
        {
            this.historyPrinter = historyPrinter;
            this.historyManager = historyManager;
            this.deskSize = deskSize;
            this.commandManager = commandManager;
        }

        public Move Turn(LatrunculiApp latrunculi, ChessBoxState player, CancellationToken ct = default)
        {
            Console.Write("Váš tah (start cíl): ");
            string line = Console.ReadLine().Trim().ToLower();
            if (commandManager.CheckCommand(line))
            {
                return null;
            }
            var moveMatch = Regex.Match(line, @"^(?<from>[a-zA-Z][1-9])\s*(?<to>[a-zA-Z][1-9])$");
            if (moveMatch.Success)
            {
                ChessBoxPosition from = ChessBoxPosition.FromString(deskSize, moveMatch.Groups["from"].Value);
                ChessBoxPosition to = ChessBoxPosition.FromString(deskSize, moveMatch.Groups["to"].Value);
                return new Move(from, to);
            }
            else
            {
                throw new InvalidCastException("Je vyžadován tah ve formátu start cíl, např. E4 E5");
            }
        }

        public override string ToString()
        {
            return $"{nameof(ConsolePlayer)}";
        }
    }
}
