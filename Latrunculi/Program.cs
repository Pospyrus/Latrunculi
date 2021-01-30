using LatrunculiCore;
using LatrunculiCore.Desk;
using LatrunculiCore.Exceptions;
using LatrunculiCore.Moves;
using LatrunculiCore.Players;
using System;
using System.Linq;
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
            DeskPrinter deskPrinter = new DeskPrinter(latrunculi.Desk, latrunculi.HistoryManager);
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
                                WriteColoredMulti(
                                    TextSegment($"Tah "),
                                    TextSegment($"@{latrunculi.HistoryManager.ActualRound}", ConsoleColor.Yellow),
                                    TextSegment($", hraje "),
                                    TextSegment($"{actualPlayer} ", ConsoleColor.Yellow),
                                    TextSegment($"hráč. "));
                                var move = latrunculi.Turn();
                                if (move != null)
                                {
                                    WriteColoredMulti(TextSegment("Zahrán tah "), TextSegment($"{move}", ConsoleColor.Green), TextSegment("."));
                                }
                                latrunculi.Rules.CheckEndOfGame(latrunculi.HistoryManager.ActualRound);
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

        public static (string text, ConsoleColor? color, ConsoleColor? backgroundColor) TextSegment(string text, ConsoleColor? color = null, ConsoleColor? backgroundColor = null) =>
            (text, color, backgroundColor);

        public static (string text, ConsoleColor? color, ConsoleColor? backgroundColor) NewLineSegment =
            (Environment.NewLine, null, null);

        public static void WriteColoredMulti(params (string text, ConsoleColor? color, ConsoleColor? backgroundColor)[] textSegments)
        {
            foreach (var segment in textSegments)
            {
                WriteColored(segment.text, segment.color, segment.backgroundColor);
            }
        }

        public static void WriteColoredLine(string text, ConsoleColor? color = null, ConsoleColor? backgroundColor = null) =>
            coloredConsoleAction(() => Console.WriteLine(text), color, backgroundColor);

        public static void WriteColored(string text, ConsoleColor? color = null, ConsoleColor? backgroundColor = null) =>
            coloredConsoleAction(() => Console.Write(text), color, backgroundColor);

        private static void coloredConsoleAction(Action action, ConsoleColor? color = null, ConsoleColor? backgroundColor = null)
        {
            var previousColor = Console.ForegroundColor;
            var previousBackgroundColor = Console.BackgroundColor;
            if (color != null)
            {
                Console.ForegroundColor = color.Value;
            }
            if (backgroundColor != null)
            {
                Console.BackgroundColor = backgroundColor.Value;
            }
            action();
            Console.ForegroundColor = previousColor;
            Console.BackgroundColor = previousBackgroundColor;
        }
    }
}
