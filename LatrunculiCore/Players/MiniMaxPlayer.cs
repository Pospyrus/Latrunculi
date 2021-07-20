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
            IEnumerable<(Move move, int evaluation, int bestMovesCount)> moves = validMoves.Select(move => (minMaxRecursive(depth, move))).ToArray();
            if (app.Debug)
            {
                logger.WriteLine("############################# Result");
                foreach (var moveInfo in moves)
                {
                    logger.WriteLine($"{moveInfo.evaluation} {moveInfo.move}, count: {moveInfo.bestMovesCount}");
                }
            }
            if (app.BestMovesDebug)
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

        private (Move move, int evaluation, int bestMovesCount) minMaxRecursive(int depth, Move move)
        {
            app.Rules.Move(app.HistoryManager.ActualPlayer, move);
            var indentation = this.depth - depth;
            var moves = string.Join(';', app.HistoryManager.Steps.TakeLast(this.depth - depth + 1)
                            .Select(step => step.Move.ToString()));
            if (app.Debug)
            {
                deskLogger.WriteLine($"{app.HistoryManager.ActualPlayer} {moves}");
                deskLogger.WriteLine(deskToString.GetDeskAsString());
            }
            (Move move, int evaluation, int bestMovesCount)? evaluation = null;
            if (depth == 0 || app.IsEnded)
            {
                evaluation = (move, evaluationPlayer(), 1);
                if (app.Debug && evaluation.Value.evaluation != 0)
                {
                    logger.WriteLine($"{app.HistoryManager.ActualPlayer} {moves} ({evaluation})", indentation);
                }
            }
            else
            {
                //logger.WriteLine($"Start {app.HistoryManager.ActualPlayer} {moves}", indentation);
                var evaluations = app.Rules.GetAllValidMoves(app.HistoryManager.ActualPlayer)
                    .ToArray()
                    .Select<Move, (Move move, int evaluation, int bestMovesCount)>(nextMove => minMaxRecursive(depth - 1, nextMove))
                    .ToArray();
                var maxEvaluation = evaluations.Max(move => move.evaluation);
                var maxCount = evaluations
                    .Where(eval => eval.evaluation == maxEvaluation)
                    .Sum(eval => eval.bestMovesCount);
                if (app.Debug && maxEvaluation != 0)
                {
                    logger.WriteLine($"End {app.HistoryManager.ActualPlayer} {moves} ({maxEvaluation}, count: {maxCount})", indentation);
                }
                evaluation = (move, maxEvaluation, maxCount);
            }
            app.HistoryManager.Back();
            return evaluation.Value;
        }

        private int evaluationPlayer()
        {
            var evaluation = app.HistoryManager.Steps.TakeLast(depth + 1).Reverse().Select((step, index) =>
                step.CapturedCount * (index + 1) * (index % 2 == 0 ? 1 : -1)
            ).Sum();
            if (app.Debug && evaluation != 0)
            {
                string steps = string.Join(", ", app.HistoryManager.Steps.TakeLast(depth + 1).Select(step => step.Move));
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