using Latrunculi;
using LatrunculiCore.Desk;
using LatrunculiCore.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LatrunculiCore.Players
{
    public class MiniMaxPlayer : IPlayer
    {
        private int depth;
        private Random random = new Random();
        private FileLogger logger;
        private FileLogger deskLogger;
        private DeskToString deskToString;

        public event EventHandler DebugPrintDesk;

        public MiniMaxPlayer(int depth)
        {
            this.depth = depth;
            deskToString = new DeskToString();
        }

        public Move Turn(LatrunculiApp latrunculi, ChessBoxState player, CancellationToken ct = default)
        {
            return getBestMove(latrunculi, player, ct);
        }

        private Move getBestMove(LatrunculiApp latrunculi, ChessBoxState player, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            logger?.Dispose();
            deskLogger?.Dispose();
            logger = new FileLogger($"{latrunculi.HistoryManager.ActualRound} - {player} - minimax {depth}");
            deskLogger = new FileLogger($"{latrunculi.HistoryManager.ActualRound} - {player} - minimax {depth} - desk");

            var validMoves = latrunculi.Rules.GetAllValidMoves(player).ToArray();
            IEnumerable<(Move move, int evaluation, int bestMovesCount)> moves = validMoves.Select(move => (minMaxRecursive(latrunculi, depth, move, ct))).ToArray();
            if (latrunculi.Debug)
            {
                logger.WriteLine("############################# Result");
                foreach (var moveInfo in moves)
                {
                    logger.WriteLine($"{moveInfo.evaluation} {moveInfo.move}, count: {moveInfo.bestMovesCount}");
                }
            }
            if (latrunculi.BestMovesDebug)
            {
                Console.WriteLine();
                foreach (var moveInfo in moves)
                {
                    Console.WriteLine($"{moveInfo.evaluation} {moveInfo.move}, count: {moveInfo.bestMovesCount}");
                }
                Console.WriteLine($"--------------------------------");
            }
            var maxEvaluation = moves.Max(move => move.evaluation);
            var maxMoves = moves.Where(move => move.evaluation == maxEvaluation).ToArray();
            var maxBestCount = maxMoves.Max(move => move.bestMovesCount);
            var bestMoves = maxMoves.Where(move => move.bestMovesCount == maxBestCount).Select(move => move.move).ToArray();
            var moveForwardDirection = player == ChessBoxState.Black ? -1 : 1;
            var bestForwardMoves = bestMoves.Where(move => move.To.Y - move.From.Y == moveForwardDirection).ToArray();
            if (bestForwardMoves.Length == 0)
            {
                bestForwardMoves = bestMoves.Where(move => move.From.Y == move.To.Y).ToArray();
            }
            if (bestForwardMoves.Length == 0)
            {
                bestForwardMoves = bestMoves;
            }
            logger.Dispose();
            return bestForwardMoves.Skip(random.Next(bestForwardMoves.Count()) - 1).First();
        }

        private (Move move, int evaluation, int bestMovesCount) minMaxRecursive(LatrunculiApp latrunculi, int depth, Move move, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                latrunculi.Rules.Move(latrunculi.HistoryManager.ActualPlayer, move);
                var indentation = this.depth - depth;
                var moves = string.Join(';', latrunculi.HistoryManager.Steps.TakeLast(this.depth - depth + 1)
                                .Select(step => step.Move.ToString()));
                if (latrunculi.Debug)
                {
                    deskLogger.WriteLine($"{latrunculi.HistoryManager.ActualPlayer} {moves}");
                    deskLogger.WriteLine(deskToString.GetDeskAsString(latrunculi));
                }
                (Move move, int evaluation, int bestMovesCount)? evaluation = null;
                if (depth == 0 || latrunculi.IsEnded)
                {
                    evaluation = (move, evaluationPlayer(latrunculi), 1);
                    if (latrunculi.Debug && evaluation.Value.evaluation != 0)
                    {
                        logger.WriteLine($"{latrunculi.HistoryManager.ActualPlayer} {moves} ({evaluation})", indentation);
                    }
                }
                else
                {
                    //logger.WriteLine($"Start {app.HistoryManager.ActualPlayer} {moves}", indentation);
                    var evaluations = latrunculi.Rules.GetAllValidMoves(latrunculi.HistoryManager.ActualPlayer)
                        .ToArray()
                        .Select(nextMove => minMaxRecursive(latrunculi, depth - 1, nextMove, ct))
                        .ToArray();
                    var maxEvaluation = evaluations.Max(move => move.evaluation);
                    var maxCount = evaluations
                        .Where(eval => eval.evaluation == maxEvaluation)
                        .Sum(eval => eval.bestMovesCount);
                    if (latrunculi.Debug && maxEvaluation != 0)
                    {
                        logger.WriteLine($"End {latrunculi.HistoryManager.ActualPlayer} {moves} ({maxEvaluation}, count: {maxCount})", indentation);
                    }
                    evaluation = (move, maxEvaluation, maxCount);
                }
                return evaluation.Value;
            }
            finally
            {
                latrunculi.HistoryManager.Back();
            }
        }

        private int evaluationPlayer(LatrunculiApp latrunculi)
        {
            var evaluation = latrunculi.HistoryManager.Steps.TakeLast(depth + 1).Reverse().Select((step, index) =>
                step.CapturedCount * (index + 1) * (index % 2 == 0 ? 1 : -1)
            ).Sum();
            if (latrunculi.Debug && evaluation != 0)
            {
                string steps = string.Join(", ", latrunculi.HistoryManager.Steps.TakeLast(depth + 1).Select(step => step.Move));
                Console.WriteLine($"{steps}: {evaluation}");
                DebugPrintDesk?.Invoke(this, null);
            }
            return depth % 2 == 0 ? evaluation : -evaluation;
        }

        public override string ToString()
        {
            return $"{nameof(MiniMaxPlayer)}({depth})";
        }
    }
}