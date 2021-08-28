using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LatrunculiCore.Desk
{
    public class Move : IEquatable<Move>
    {
        [JsonInclude]
        public ChessBoxPosition From { get; private set; }
        
        [JsonInclude]
        public ChessBoxPosition To { get; private set; }

        [JsonIgnore]
        public string HashCode => $"{From.Letter}{From.Number} {To.Letter}{To.Number}";

        public override string ToString() => HashCode;

        public bool Equals([AllowNull] Move other) => this == other;
        public static bool operator ==(Move a, Move b)
            => a?.HashCode == b?.HashCode;

        public static bool operator !=(Move a, Move b)
            => a?.HashCode != b?.HashCode;

        [JsonConstructor]
        public Move(ChessBoxPosition from, ChessBoxPosition to)
        {
            From = from;
            To = to;
        }
    }
}
