using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domain
{
    
    [Serializable]
    public class BrickConfiguration
    {

        public string name;
        
        public Color color;
        
        public List<BrickCellConfiguration> cells;

        public override string ToString()
        {
            var res = $"> Brick  {name}\n";
            
            res += $" - Color {color}\n";
            
            res += $" - " + (cells == null ? "No cells " : $"Cells size is {cells.Count}");
            if (cells != null)
                cells.ForEach(configuration => res += $"\n{configuration}");

            return res;
        }
    }
}