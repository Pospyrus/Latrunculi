using System;

namespace LatrunculiCore.Desk
{
    public class DeskSize
    {
        public readonly int Width;
        public readonly int Height;

        public int Count => Width * Height;

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
    }
}
