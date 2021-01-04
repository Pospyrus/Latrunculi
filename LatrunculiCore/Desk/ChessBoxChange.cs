using System;
using System.Collections.Generic;
using System.Text;

namespace LatrunculiCore.Desk
{
    public class ChessBoxChange
    {
        public ChessBoxReference CheckBox { get; set; }
        public ChessBoxState OldState { get; set; }
        public ChessBoxState NewState { get; set; }
    }
}
