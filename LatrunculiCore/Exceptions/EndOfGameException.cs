using System;
using LatrunculiCore.Desk;

namespace LatrunculiCore.Exceptions
{
    public class EndOfGameException : Exception
    {
        public readonly ChessBoxState Winner;

        public EndOfGameException(string message, ChessBoxState winner)
            : base(message)
        {
            Winner = winner;
        }
    }
}