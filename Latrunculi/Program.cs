using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Exceptions;
using LatrunculiCore.Moves;
using LatrunculiCore.Players;
using System;
using System.Text.RegularExpressions;

namespace Latrunculi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Latrunculi - @author: František Pospíšil");
            Console.WriteLine("========================================");
            Console.WriteLine();

            var latrunculi = new LatrunculiApp();
            HistoryPrinter historyPrinter = new HistoryPrinter(latrunculi.HistoryManager);
            var commandManager = new CommandManager(latrunculi, historyPrinter);
            DeskPrinter deskPrinter = new DeskPrinter(latrunculi.Desk);
            latrunculi.WhitePlayer = commandManager.GetPlayerType(ChessBoxState.White);
            latrunculi.BlackPlayer = commandManager.GetPlayerType(ChessBoxState.Black);

            while (true)
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            if (!latrunculi.IsEnded)
                            {
                                Console.WriteLine();
                                deskPrinter.PrintDesk();
                                string actualPlayer = latrunculi.HistoryManager.ActualPlayer == ChessBoxState.Black ? "černý" : "bílý";
                                Console.WriteLine();
                                Console.Write($"Tah ");
                                WriteColored($"@{latrunculi.HistoryManager.ActualRound + 2}", ConsoleColor.Yellow);
                                Console.Write($", hraje ");
                                WriteColored($"{actualPlayer} ", ConsoleColor.Yellow);
                                Console.Write($"hráč. ");
                                var move = latrunculi.Turn();
                                if (move != null)
                                {
                                    Console.Write($"Zahrán tah. ");
                                    WriteColoredLine($"{move}", ConsoleColor.Green);
                                }
                            }
                            else
                            {
                                Console.Write("Zadejte příkaz: ");
                                var line = Console.ReadLine();
                                commandManager.CheckCommand(line);
                            }
                        }
                        catch (EndOfGameException e)
                        {
                            Console.WriteLine();
                            deskPrinter.PrintDesk();
                            WriteColoredLine(e.Message, ConsoleColor.Green);
                            break;
                        }
                        catch (AbortGameException)
                        {
                            return;
                        }
                        catch (Exception e)
                        {
                            WriteColoredLine(e.Message, ConsoleColor.Red);
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public static void WriteColoredLine(string text, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = previousColor;
        }

        public static void WriteColored(string text, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = previousColor;
        }
    }
}
