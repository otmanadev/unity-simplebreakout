using System;

namespace Domain
{
    
    [Serializable]
    public class BrickCellConfiguration
    {

        public int x;

        public int y;

        public override string ToString()
        {
            return $"  > Brick Cell [{x}, {y}]";
        }
    }
}