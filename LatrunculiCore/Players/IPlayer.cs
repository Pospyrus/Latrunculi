using LatrunculiCore.Desk;
using System.Threading;

namespace LatrunculiCore.Players
{
    public interface IPlayer
    {
        Move Turn(ChessBoxState player, CancellationToken ct = default);
    }
}
