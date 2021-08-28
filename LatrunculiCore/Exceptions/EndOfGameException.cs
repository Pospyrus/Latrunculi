using System;
using LatrunculiCore.Desk;

namespace LatrunculiCore.Exceptions
{
    public class EndOfGameException : Exception
    {
        public ChessBoxState Winner { get; private set; }

        public EndOfGameException(string message, ChessBoxState winner)
            : base(message)
        {
            Winner = winner;
        }
    }
}