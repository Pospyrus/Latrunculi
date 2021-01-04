using LatrunculiCore.Desk;
using System;
using System.Text.RegularExpressions;

namespace Latrunculi
{
    class Program
    {
        static void Main(string[] args)
        {
            var deskSize = new DeskSize(8, 7);
            Desk desk = new Desk(deskSize);
            var historyManager = new DeskHistoryManager(desk);
            desk.Moved += (_, step) => historyManager.Add(step);
            new DeskSpawner(desk).Spawn();
            DeskPrinter deskPrinter = new DeskPrinter(desk);
            HistoryPrinter historyPrinter = new HistoryPrinter(historyManager);
            while (true)
            {
                deskPrinter.PrintDesk();
                Console.WriteLine();
                string actualPlayer = historyManager.Index % 2 == 0 ? "černý" : "bílý";
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
                        ChessBoxReference from = ChessBoxReference.FromString(deskSize, moveMatch.Groups["from"].Value);
                        ChessBoxReference to = ChessBoxReference.FromString(deskSize, moveMatch.Groups["to"].Value);
                        desk.Move(from, to);
                    }
                    else
                    {
                        throw new InvalidCastException("Je vyžadován tah ve formátu start cíl, např. E4 E5");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
