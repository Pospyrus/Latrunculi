using System;
using System.Text.Json.Serialization;

namespace LatrunculiCore.Desk
{
    public class DeskSize
    {
        [JsonInclude]
        public int Width { get; private set; }

        [JsonInclude]
        public int Height { get; private set; }

        [JsonIgnore]
        public int Count => Width * Height;

        [JsonConstructor]
        public DeskSize(int width, int height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), $"Size must be positive.");
            }
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), $"Size must be positive.");
            }
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"DeskSize {Width}x{Height}";
        }
    }
}
