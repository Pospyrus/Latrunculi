using System;
using System.Linq;
using System.Threading;
using LatrunculiCore.Desk;

namespace LatrunculiCore.Players
{
    public class RandomPlayer : IPlayer
    {
        private Random random = new Random();

        public Move Turn(LatrunculiApp latrunculi, ChessBoxState player, CancellationToken ct = default)
        {
            var validMoves = latrunculi.Rules.GetAllValidMoves(player);
            return validMoves.Skip(random.Next(validMoves.Count()) - 1).First();
        }

        public override string ToString()
        {
            return nameof(RandomPlayer);
        }
    }
}
