﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LatrunculiCore.Desk
{
    public class AllPositions: IEnumerable<ChessBoxPosition>
    {
        public ChessBoxPosition[] Positions { get; private set; }

        public AllPositions(DeskSize size)
        {
            Positions = new ChessBoxPosition[size.Width * size.Height];
            foreach (var x in Enumerable.Range(0, size.Width))
            {
                foreach (var y in Enumerable.Range(0, size.Height))
                {
                    var position = new ChessBoxPosition(size, x, y);
                    Positions[position.Index] = position;
                }
            }
        }

        public IEnumerator<ChessBoxPosition> GetEnumerator()
        {
            return Positions.Cast<ChessBoxPosition>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Positions.GetEnumerator();
        }

        public void Dispose()
        {
            foreach (var position in Positions)
            {
                position.Dispose();
            }
            Positions = null;
        }
    }
}
