using LatrunculiCore.Desk;
using LatrunculiCore.Moves;
using LatrunculiCore.Players;

namespace LatrunculiCore
{
    public class LatrunculiApp
    {
        public readonly DeskManager Desk;
        public readonly AllPositions AllPositions;
        public readonly DeskHistoryManager HistoryManager;
        public readonly RulesManager Rules;

        public IPlayer WhitePlayer;
        public IPlayer BlackPlayer;

        public IPlayer ActualPlayer => HistoryManager.ActualPlayer == ChessBoxState.Black ? BlackPlayer : WhitePlayer;

        public bool IsEnded => HistoryManager.ActualRound > 28;

        public LatrunculiApp() {
            var deskSize = new DeskSize(8, 7);            
            AllPositions = new AllPositions(deskSize);
            Desk = new DeskManager(deskSize);
            Rules = new RulesManager(Desk, AllPositions);
            HistoryManager = new DeskHistoryManager();
            HistoryManager.GoingPrev += (_, changes) => Desk.DoStep(changes);
            HistoryManager.GoingBack += (_, changes) => Desk.RevertStep(changes);
            Rules.Moved += (_, step) => HistoryManager.Add(step);
            new DeskSpawner(Desk).Spawn();
        }

        public Move Turn() {
            var move = this.ActualPlayer.Turn(HistoryManager.ActualPlayer);
            if (move != null) {
                Rules.Move(HistoryManager.ActualPlayer, move, HistoryManager.ActualRound);
            }
            return move;
        }
    }
}
