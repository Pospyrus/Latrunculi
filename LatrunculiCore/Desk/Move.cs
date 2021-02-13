using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LatrunculiCore.Desk
{
    public class Move : IEquatable<Move>
    {
        [JsonInclude]
        public readonly ChessBoxPosition From;
        
        [JsonInclude]
        public readonly ChessBoxPosition To;

        [JsonIgnore]
        public string HashCode => $"{From.Letter}{From.Number} {To.Letter}{To.Number}";

        public override string ToString() => HashCode;

        public bool Equals([AllowNull] Move other) => HashCode == other?.HashCode;

        [JsonConstructor]
        public Move(ChessBoxPosition from, ChessBoxPosition to)
        {
            From = from;
            To = to;
        }
    }
}
