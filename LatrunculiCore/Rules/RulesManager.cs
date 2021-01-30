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

        public event EventHandler<ChangeSet> Moved;

        public int MaxRoundsCount => 30;

        public RulesManager(DeskManager desk, AllPositions allPositions)
        {
            this.desk = desk;
            this.allPositions = allPositions;
        }

        public bool Move(ChessBoxState actualPlayer, Move move)
        {
            ValidateMove(actualPlayer, move);
            ChangeSet step = new ChangeSet();
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
            foreach (var position in getPositionsByPlayer(actualPlayer))
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

        public void CheckEndOfGame(int actualRound)
        {
            var blackCount = getPositionsByPlayer(ChessBoxState.Black).Count();
            var whiteCount = getPositionsByPlayer(ChessBoxState.White).Count();
            if (blackCount == 0)
            {
                throw new EndOfGameException("Konec hry. Vyhrává BÍLÝ hráč!", ChessBoxState.White);
            }
            if (whiteCount == 0)
            {
                throw new EndOfGameException("Konec hry. Vyhrává ČERNÝ hráč!", ChessBoxState.Black);
            }
            if (actualRound > MaxRoundsCount)
            {
                if (blackCount > whiteCount)
                {
                    throw new EndOfGameException($"Vypršel počet kol. Vyhrává ČERNÝ hráč s {blackCount} kameny!!", ChessBoxState.Black);
                }
                if (blackCount > whiteCount)
                {
                    throw new EndOfGameException($"Vypršel počet kol. Vyhrává BÍLÝ hráč s {whiteCount} kameny!", ChessBoxState.Black);
                }
                throw new EndOfGameException($"Vypršel počet kol. REMÍZA, každému hráči zůstalo {whiteCount} kamenů!", ChessBoxState.Black);
            }
        }

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

        private IEnumerable<ChessBoxPosition> getPositionsByPlayer(ChessBoxState actualPlayer)
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
