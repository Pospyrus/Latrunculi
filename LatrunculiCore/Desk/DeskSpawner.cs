using System.Linq;

namespace LatrunculiCore.Desk
{
    public class DeskSpawner
    {
        private DeskManager desk;

        public DeskSpawner(DeskManager desk)
        {
            this.desk = desk;
        }

        public void Spawn()
        {
            fillRow(0, ChessBoxState.White);
            fillRow(1, ChessBoxState.White);
            fillRow(desk.Size.Height - 1, ChessBoxState.Black);
            fillRow(desk.Size.Height - 2, ChessBoxState.Black);
        }

        private void fillRow(int y, ChessBoxState state)
        {
            foreach (var x in Enumerable.Range(0, desk.Size.Width))
            {
                desk.PlayingDesk[x, y] = state;
            }
        }
    }
}
