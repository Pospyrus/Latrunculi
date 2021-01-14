namespace LatrunculiCore.Desk
{
    public class ChessBoxChange
    {
        public ChessBoxPosition Position { get; set; }
        public ChessBoxState OldState { get; set; }
        public ChessBoxState NewState { get; set; }
    }
}
