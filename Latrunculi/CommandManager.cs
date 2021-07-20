using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Players;

namespace Latrunculi
{
    public class CommandManager
    {
        private HistoryPrinter historyPrinter;
        private LatrunculiApp app;
        private DeskPrinter deskPrinter;
        private IPlayer helpPlayer;

        public CommandManager(LatrunculiApp app, HistoryPrinter historyPrinter, DeskPrinter deskPrinter)
        {
            this.app = app;
            this.historyPrinter = historyPrinter;
            this.deskPrinter = deskPrinter;
            this.playerTypes = new Dictionary<string, Func<IPlayer>>()
            {
                ["human"] = () => createHumanPlayer(),
                ["random"] = () => createRandomPlayer(),
                ["minimax2"] = () => createMiniMaxPlayer(2),
                ["minimax4"] = () => createMiniMaxPlayer(4),
                ["minimax3"] = () => createMiniMaxPlayer(3)
            };
            this.helpPlayer = createMiniMaxPlayer(3);
        }

        public bool CheckCommand(string line)
        {
            line = line?.ToLower();
            if (line == "history")
            {
                historyPrinter.PrintHistory();
                Console.WriteLine("Stiskni libovolnou klávesu...");
                Console.ReadKey();
                Console.WriteLine();
                return true;
            }
            var debugModeMatch = Regex.Match(line, @"debug (?<value>(true|1|on|false|0|off))");
            if (debugModeMatch.Success)
            {
                string value = debugModeMatch.Groups["value"].Value.ToLower();
                app.Debug = value == "true" || value == "1" || value == "on";
                return true;
            }
            var historyStepMatch = Regex.Match(line, @"^history\s+(?<direction>(prev|back))");
            if (historyStepMatch.Success)
            {
                string direction = historyStepMatch.Groups["direction"].Value;
                if (direction == "prev")
                    app.HistoryManager.Prev();
                else if (direction == "back")
                    app.HistoryManager.Back();
                return true;
            }
            var helpMatch = Regex.Match(line, @"^help(\s+?<playerType>(\w+)?)?");
            if (helpMatch.Success)
            {
                var playerType = helpMatch.Groups["playerType"].Value?.ToLower();
                var player = helpPlayer;
                if (playerType != null && playerTypes.TryGetValue(playerType, out var playerFactory) && playerFactory != null)
                {
                    player = playerFactory();
                }
                var bestMove = player.Turn(app.HistoryManager.ActualPlayer);
                if (bestMove != null)
                {
                    Program.WriteColoredMulti(
                        Program.TextSegment("Dobrý tah by mohl být "),
                        Program.TextSegment(bestMove.ToString(), ConsoleColor.Yellow),
                        Program.TextSegment("."),
                        Program.NewLineSegment
                    );
                }
                else
                {
                    Console.WriteLine("Promiň. Nevím jak hrát.");
                }
                Console.WriteLine("Stiskni libovolnou klávesu...");
                Console.ReadKey();
                return true;
            }
            var historyMatch = Regex.Match(line, @"^history\s+(?<historyIndex>\d+)$");
            if (historyMatch.Success)
            {
                if (int.TryParse(historyMatch.Groups["historyIndex"].Value, out int newHistoryIndex))
                {
                    app.HistoryManager.GoTo(newHistoryIndex - 1);
                }
                return true;
            }
            var saveMatch = Regex.Match(line, @"^save\s+(?<name>.*)");
            if (saveMatch.Success)
            {
                var name = saveMatch.Groups["name"].Value.Trim();
                if (app.SaveGameManager.SaveToFile(name))
                {
                    Program.WriteColoredLine($"Hra {name} byla uložena.", ConsoleColor.Green);
                }
                return true;
            }
            var loadMatch = Regex.Match(line, @"^load(\s+(?<name>.*))?");
            if (loadMatch.Success)
            {
                var name = loadMatch.Groups["name"].Value;
                if (string.IsNullOrEmpty(name))
                {
                    var games = string.Join('\n', app.SaveGameManager.GetSavedGamesList());
                    Console.WriteLine();
                    Program.WriteColoredLine($"Seznam uložených her:");
                    Program.WriteColoredLine("===============================");
                    Program.WriteColoredLine(games, ConsoleColor.Green);
                    Console.ReadKey();
                    return true;
                }
                if (app.SaveGameManager.LoadFromFile(name))
                {
                    Program.WriteColoredLine($"Hra {name} byla načtena.", ConsoleColor.Green);
                }
                return true;
            }
            var changePlayerTypeMatch = Regex.Match(line, @"^\s*player\s+(?<player>\w+)\s+(?<playerType>\w+)\s*$");
            if (changePlayerTypeMatch.Success)
            {
                var player = changePlayerTypeMatch.Groups["player"].Value;
                var playerType = changePlayerTypeMatch.Groups["playerType"].Value;
                
                var newPlayer = createPlayerByType(playerType);
                if (newPlayer == null)
                {
                    Program.WriteColoredLine($"Typ hráče {playerType} neexistuje.", ConsoleColor.Red);
                    return true;
                }

                player = player?.Trim().ToLower();
                if (player == "black")
                {
                    app.BlackPlayer = newPlayer;
                }
                else if (player == "white")
                {
                    app.WhitePlayer = newPlayer;
                }
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
                if (this.playerTypes.ContainsKey(input))
                {
                    return true;
                }
                Program.WriteColoredLine("Zadán chybný typ hráče.", ConsoleColor.Red);
                return false;
            });
            return createPlayerByType(playerType);
        }

        public IPlayer createPlayerByType(string playerType)
        {
            playerType = playerType?.Trim().ToLower();
            if (playerType == null || !this.playerTypes.ContainsKey(playerType))
            {
                return null;
            }
            return playerTypes[playerType]();
        }

        private Dictionary<string, Func<IPlayer>> playerTypes;

        private IPlayer createHumanPlayer() =>
            new ConsolePlayer(historyPrinter, app.HistoryManager, app.Desk.Size, this);

        private IPlayer createRandomPlayer() =>
            new RandomPlayer(app.Rules);

        private IPlayer createMiniMaxPlayer(int depth)
        {
            var player = new MiniMaxPlayer(depth, app);
            player.DebugPrintDesk += (_, __) =>
            {
                deskPrinter.PrintDesk();
            };
            return player;
        }

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