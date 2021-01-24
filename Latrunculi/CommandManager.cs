using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Moves;
using LatrunculiCore.Players;

namespace Latrunculi
{
    public class CommandManager
    {
        private HistoryPrinter historyPrinter;
        private LatrunculiApp app;

        public CommandManager(LatrunculiApp app, HistoryPrinter historyPrinter)
        {
            this.app = app;
            this.historyPrinter = historyPrinter;
            this.playerTypes = new Dictionary<string, Func<IPlayer>>()
            {
                ["human"] = () => createHumanPlayer,
                ["random"] = () => createRandomPlayer
            };
        }

        public bool CheckCommand(string line)
        {
            if (line == "history")
            {
                historyPrinter.PrintHistory();
                Console.WriteLine("Stiskni libovolnou klávesu...");
                Console.ReadKey();
                Console.WriteLine();
                return true;
            }
            var historyStepMatch = Regex.Match(line, @"^history (?<direction>(prev|back))");
            if (historyStepMatch.Success)
            {
                string direction = historyStepMatch.Groups["direction"].Value;
                if (direction == "prev")
                    app.HistoryManager.Prev();
                else if (direction == "back")
                    app.HistoryManager.Back();
                return true;
            }
            var historyMatch = Regex.Match(line, @"^history\s+(?<historyIndex>\d+)$");
            if (historyMatch.Success)
            {
                if (int.TryParse(historyMatch.Groups["historyIndex"].Value, out int newHistoryIndex))
                    app.HistoryManager.GoTo(newHistoryIndex - 1);
                return true;
            }
            return false;
        }

        public IPlayer GetPlayerType(ChessBoxState player)
        {
            var playerName = player == ChessBoxState.Black ? "černého" : "bílého";
            var playerTypesNames = string.Join(", ", playerTypes.Keys);
            var playerType = GetValidValue($"Zadejte typ {playerName} hráče ({playerTypesNames}): ", (string input) =>
            {
                input = input.Trim().ToLower();
                if (this.playerTypes.ContainsKey(input)){
                    return true;
                }
                Program.WriteColoredLine("Zadán chybný typ hráče.", ConsoleColor.Red);
                return false;
            });
            return playerTypes[playerType]();
        }

        private Dictionary<string, Func<IPlayer>> playerTypes;

        private IPlayer createHumanPlayer =>
            new ConsolePlayer(historyPrinter, app.HistoryManager, app.Desk.Size, this);

        private IPlayer createRandomPlayer =>
            new RandomPlayer(app.Rules);

        public string GetValidValue(string message, Func<string, bool> validateAction)
        {
            while (true)
            {
                Console.Write(message);
                var input = Console.ReadLine();
                if (validateAction(input))
                {
                    return input;
                }
            }
        }
    }
}