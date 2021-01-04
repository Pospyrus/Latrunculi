using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatrunculiCore.Desk
{
    public class DeskSpawner
    {
        private Desk desk;

        public DeskSpawner(Desk desk)
        {
            this.desk = desk;
        }

        public void Spawn()
        {
            fillRow(0, ChessBoxState.WhitePawn);
            fillRow(1, ChessBoxState.WhitePawn);
            fillRow(desk.Size.Y - 1, ChessBoxState.BlackPawn);
            fillRow(desk.Size.Y - 2, ChessBoxState.BlackPawn);
        }

        private void fillRow(int y, ChessBoxState state)
        {
            foreach (var x in Enumerable.Range(0, desk.Size.X))
            {
                desk.PlayingDesk[x, y] = state;
            }
        }
    }
}
