using System;
using System.Linq;
using System.Threading;
using LatrunculiCore.Desk;
using LatrunculiCore.Moves;

namespace LatrunculiCore.Players
{
    public class RandomPlayer : IPlayer
    {
        private RulesManager rulesManager;
        private Random random = new Random();

        public RandomPlayer(RulesManager rulesManager) {
            this.rulesManager = rulesManager;
        }

        public Move Turn(ChessBoxState player, CancellationToken ct = default)
        {
            var validMoves = rulesManager.GetAllValidMoves(player);
            return validMoves.Skip(random.Next(validMoves.Count()) - 1).First();
        }

        public override string ToString()
        {
            return nameof(RandomPlayer);
        }
    }
}
