using LatrunculiCore.Desk;
using LatrunculiCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LatrunculiCore.Moves
{
    public class RulesManager
    {
        private DeskManager desk;
        private AllPositions allPositions;
        private DeskHistoryManager historyManager;

        public event EventHandler<ChangeSet> Moved;

        public int MaxEmptyRoundsCount => 30;

        public RulesManager(DeskManager desk, AllPositions allPositions, DeskHistoryManager historyManager)
        {
            this.desk = desk;
            this.allPositions = allPositions;
            this.historyManager = historyManager;
        }

        public bool Move(ChessBoxState actualPlayer, Move move)
        {
            ValidateMove(actualPlayer, move);
            ChangeSet step = new ChangeSet(move);
            ChessBoxState stateFrom = desk.GetState(move.From);
            ChessBoxState stateTo = desk.GetState(move.To);
            step.AddChange(move.From, stateFrom, ChessBoxState.Empty);
            step.AddChange(move.To, stateTo, stateFrom);
            step.AddChangesRange(MoveEffects(actualPlayer, move));
            desk.DoStep(step);
            Moved?.Invoke(this, step);
            return true;
        }

        public void ValidateMove(ChessBoxState actualPlayer, Move move)
        {
            if (GetAllValidMoves(actualPlayer).Contains(move))
            {
                return;
            }
            if (desk.GetState(move.From) != actualPlayer)
            {
                var actualPlayerText = actualPlayer == ChessBoxState.Black ? "černý" : "bílý";
                throw new Exception($"Tah není platný, protože na tahu je {actualPlayerText} hráč.");
            }
            throw new Exception($"Tah není platný.");
        }

        public IEnumerable<Move> GetAllValidMoves(ChessBoxState actualPlayer)
        {
            foreach (var position in GetPositionsByPlayer(actualPlayer))
            {
                foreach (var move in getValidMovesFromPosition(position))
                {
                    yield return move;
                }
            }
        }

        public IEnumerable<ChessBoxChange> MoveEffects(ChessBoxState actualPlayer, Move move)
        {
            foreach (var enemyToRemove in getEnemiesToRemove(actualPlayer, move))
            {
                yield return new ChessBoxChange()
                {
                    Position = enemyToRemove,
                    OldState = desk.GetState(enemyToRemove),
                    NewState = ChessBoxState.Empty,
                    IsCaptured = true
                };
            }
        }

        public void CheckEndOfGame()
        {
            var blackCount = GetPositionsByPlayer(ChessBoxState.Black).Count();
            var whiteCount = GetPositionsByPlayer(ChessBoxState.White).Count();
            if (blackCount == 0)
            {
                throw new EndOfGameException("Konec hry. Vyhrává BÍLÝ hráč!", ChessBoxState.White);
            }
            if (whiteCount == 0)
            {
                throw new EndOfGameException("Konec hry. Vyhrává ČERNÝ hráč!", ChessBoxState.Black);
            }
            if (isEndByEmptyRounds())
            {
                if (blackCount > whiteCount)
                {
                    throw new EndOfGameException($"Konec hry. Již {MaxEmptyRoundsCount} počet kol nikdo nevyhodil figurku. Vyhrává ČERNÝ hráč s {blackCount} kameny!", ChessBoxState.Black);
                }
                if (blackCount > whiteCount)
                {
                    throw new EndOfGameException($"Konec hry. Již {MaxEmptyRoundsCount} počet kol nikdo nevyhodil figurku. Vyhrává BÍLÝ hráč s {whiteCount} kameny!", ChessBoxState.White);
                }
                throw new EndOfGameException($"Konec hry. Již {MaxEmptyRoundsCount} počet kol nikdo nevyhodil figurku. REMÍZA, každému hráči zůstalo {whiteCount} kamenů!", ChessBoxState.Black);
            }
        }

        private bool isEndByEmptyRounds()
            => historyManager.ActualRound > MaxEmptyRoundsCount && !historyManager.Steps.Take(historyManager.HistoryIndex).TakeLast(MaxEmptyRoundsCount).Any(step => step.CapturedCount > 0);

        private IEnumerable<ChessBoxPosition> getEnemiesToRemove(ChessBoxState actualPlayer, Move move)
        {
            return getEnemiesToRemoveByDirection(actualPlayer, move.To, NeighborDirection.Horizontal)
                .Concat(getEnemiesToRemoveByDirection(actualPlayer, move.To, NeighborDirection.Vertical));
        }

        private IEnumerable<ChessBoxPosition> getEnemiesToRemoveByDirection(ChessBoxState actualPlayer, ChessBoxPosition position, NeighborDirection direction)
        {
            var enemyPlayer = getEnemyPlayer(actualPlayer);
            foreach (var enemy in getNeighborsByPlayer(enemyPlayer, position, direction))
            {
                if (enemy.IsCorner && getNeighborsByPlayer(actualPlayer, enemy).Count() == 1 ||
                    getNeighborsByPlayer(actualPlayer, enemy, direction).Count() == 1)
                {
                    yield return enemy;
                }
            }
        }

        private IEnumerable<ChessBoxPosition> getNeighborsByPlayer(ChessBoxState player, ChessBoxPosition position, NeighborDirection direction = NeighborDirection.All)
        {
            return from ChessBoxPosition neighborPosition in position.GetNeighbors(direction)
                   where desk.GetState(neighborPosition) == player
                   select neighborPosition;
        }

        private ChessBoxState getEnemyPlayer(ChessBoxState player) => player == ChessBoxState.Black ? ChessBoxState.White : ChessBoxState.Black;

        public IEnumerable<ChessBoxPosition> GetPositionsByPlayer(ChessBoxState actualPlayer)
        {
            return from ChessBoxPosition position in allPositions
                   where desk.GetState(position) == actualPlayer
                   select position;
        }

        private IEnumerable<Move> getValidMovesFromPosition(ChessBoxPosition position)
        {
            return from ChessBoxPosition newPosition in position.GetNeighbors()
                   where desk.GetState(newPosition) == ChessBoxState.Empty
                   select new Move(position, newPosition);
        }
    }
}
