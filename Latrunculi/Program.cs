using LatrunculiCore.Desk;
using LatrunculiCore.Moves;
using System;
using System.Text.RegularExpressions;

namespace Latrunculi
{
    class Program
    {
        static void Main(string[] args)
        {
            var deskSize = new DeskSize(8, 7);            
            var allReferences = new AllPositions(deskSize);
            DeskManager desk = new DeskManager(deskSize);
            var rules = new RulesManager(desk, allReferences);
            var historyManager = new DeskHistoryManager();
            historyManager.GoingPrev += (_, changes) => desk.DoStep(changes);
            historyManager.GoingBack += (_, changes) => desk.RevertStep(changes);
            rules.Moved += (_, step) => historyManager.Add(step);                
            new DeskSpawner(desk).Spawn();
            DeskPrinter deskPrinter = new DeskPrinter(desk);
            HistoryPrinter historyPrinter = new HistoryPrinter(historyManager);
            while (true)
            {
                deskPrinter.PrintDesk();
                Console.WriteLine();
                string actualPlayer = historyManager.ActualPlayer == ChessBoxState.Black ? "černý" : "bílý";
                Console.Write($"Tah @{historyManager.Index + 2}, hraje {actualPlayer} hráč (start cíl): ");
                string line = Console.ReadLine().Trim().ToLower();
                try
                {
                    if (line == "history")
                    {
                        historyPrinter.PrintHistory();
                        Console.WriteLine("Stiskni libovolnou klávesu...");
                        Console.ReadKey();
                        Console.WriteLine();
                        continue;
                    }
                    var historyStepMatch = Regex.Match(line, @"^history (?<direction>(prev|back))");
                    if (historyStepMatch.Success)
                    {
                        string direction = historyStepMatch.Groups["direction"].Value;
                        if (direction == "prev")
                            historyManager.Prev();
                        else if (direction == "back")
                            historyManager.Back();
                        continue;
                    }
                    var historyMatch = Regex.Match(line, @"^history\s+(?<historyIndex>\d+)$");
                    if (historyMatch.Success)
                    {
                        if (int.TryParse(historyMatch.Groups["historyIndex"].Value, out int newHistoryIndex))
                            historyManager.GoTo(newHistoryIndex - 1);
                        continue;
                    }
                    var moveMatch = Regex.Match(line, @"^(?<from>[a-zA-Z][1-9])\s*(?<to>[a-zA-Z][1-9])$");
                    if (moveMatch.Success)
                    {
                        ChessBoxPosition from = ChessBoxPosition.FromString(deskSize, moveMatch.Groups["from"].Value);
                        ChessBoxPosition to = ChessBoxPosition.FromString(deskSize, moveMatch.Groups["to"].Value);
                        var move = new Move(from, to);
                        rules.Move(historyManager.ActualPlayer, move);
                    }
                    else
                    {
                        throw new InvalidCastException("Je vyžadován tah ve formátu start cíl, např. E4 E5");
                    }
                }
                catch (Exception e)
                {
                    var previousColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = previousColor;
                }
            }
        }
    }
}
