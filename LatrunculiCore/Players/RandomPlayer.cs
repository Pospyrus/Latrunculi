using System;
using System.Linq;
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

        public Move Turn(ChessBoxState player)
        {
            var validMoves = rulesManager.GetAllValidMoves(player);
            return validMoves.Skip(random.Next(validMoves.Count()) - 1).First();
        }
    }
}
