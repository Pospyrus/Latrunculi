using LatrunculiCore.Desk;

namespace LatrunculiCore.Players
{
    public interface IPlayer
    {
        Move Turn(ChessBoxState player);
    }
}
