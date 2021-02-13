using Latrunculi;
using LatrunculiCore.Desk;
using LatrunculiCore.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LatrunculiCore.Players
{
    public class MiniMaxPlayer : IPlayer
    {
        private LatrunculiApp app;
        private int depth;
        private Random random = new Random();
        private FileLogger logger;
        private FileLogger deskLogger;
        private DeskToString deskToString;

        public event EventHandler DebugPrintDesk;

        public MiniMaxPlayer(int depth, LatrunculiApp app)
        {
            this.app = app;
            this.depth = depth;
            this.deskToString = new DeskToString(app.Desk, app.HistoryManager);
        }

        public Move Turn(ChessBoxState player)
        {
            return getBestMove(player);
        }

        private Move getBestMove(ChessBoxState player)
        {
            logger?.Dispose();
            deskLogger?.Dispose();
            logger = new FileLogger($"{app.HistoryManager.ActualRound} - {player} - minimax {depth}");
            deskLogger = new FileLogger($"{app.HistoryManager.ActualRound} - {player} - minimax {depth} - desk");
            var validMoves = app.Rules.GetAllValidMoves(player).ToArray();
            IEnumerable<(int evaluation, Move move)> moves = validMoves.Select(move => (minMaxRecursive(depth, move), move)).ToArray();
            Console.WriteLine();
            logger.WriteLine("############################# Result");
            foreach (var moveInfo in moves)
            {
                logger.WriteLine($"{moveInfo.evaluation} {moveInfo.move}");
            }
            Console.WriteLine($"--------------------------------");
            var maxEvaluation = moves.Select(move => move.evaluation).Max();
            var bestMoves = moves.Where(move => move.evaluation == maxEvaluation).Select(moveInfo => moveInfo.move);
            logger.Dispose();
            return bestMoves.Skip(random.Next(bestMoves.Count()) - 1).First();
        }

        private int minMaxRecursive(int depth, Move move)
        {
            app.Rules.Move(app.HistoryManager.ActualPlayer, move);
            var indentation = this.depth - depth;
            var moves = string.Join(';', app.HistoryManager.Steps.TakeLast(this.depth - depth + 1)
                            .Select(step => step.Move.ToString()));
            deskLogger.WriteLine($"{app.HistoryManager.ActualPlayer} {moves}");
            deskLogger.WriteLine(deskToString.GetDeskAsString());
            int evaluation = int.MinValue;
            if (depth == 0 || app.IsEnded)
            {
                evaluation = evaluationPlayer();
                if (evaluation != 0) {
                    logger.WriteLine($"{app.HistoryManager.ActualPlayer} {moves} ({evaluation})", indentation);
                }
            }
            else
            {
                //logger.WriteLine($"Start {app.HistoryManager.ActualPlayer} {moves}", indentation);
                var evaluations = app.Rules.GetAllValidMoves(app.HistoryManager.ActualPlayer)
                    .ToArray()
                    .Select<Move, (Move move, int evaluation)>(nextMove => (nextMove, -minMaxRecursive(depth - 1, nextMove)))
                    .ToArray();
                evaluation = -evaluations.Max(e => e.evaluation);
                if (evaluation != 0)
                {
                    logger.WriteLine($"End {app.HistoryManager.ActualPlayer} {moves} ({evaluation})", indentation);
                }
            }
            app.HistoryManager.Back();
            return evaluation;
        }

        private int evaluationPlayer()
        {
            var evaluation = app.HistoryManager.Steps.TakeLast(depth + 1).Reverse().Select((step, index) =>
                step.CapturedCount * (index + 1) * (index % 2 == 0 ? -1 : 1)
            ).Sum();
            if (app.Debug && evaluation != 0)
            {
                DebugPrintDesk?.Invoke(this, null);
                //     string steps = string.Join(", ", app.HistoryManager.Steps.TakeLast(depth + 1).Select(step => step.move));
                //     Console.WriteLine($"{steps}: {evaluation}");
                //     DebugPrintDesk?.Invoke(this, null);
            }
            return evaluation;
        }
    }
}