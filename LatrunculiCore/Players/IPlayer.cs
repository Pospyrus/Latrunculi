using LatrunculiCore.Desk;
using System.Threading;

namespace LatrunculiCore.Players
{
    public interface IPlayer
    {
        Move Turn(LatrunculiApp latrunculi, ChessBoxState player, CancellationToken ct = default);
    }
}
