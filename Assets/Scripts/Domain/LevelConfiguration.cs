using System;
using System.Collections.Generic;

namespace Domain
{
    
    [Serializable]
    public class LevelConfiguration
    {

        public List<BrickConfiguration> bricks;

        public override string ToString()
        {
            var res = "LevelConfiguration datas... \n";
            
            res += bricks == null ? "Bricks is null" : $"Bricks size is {bricks.Count}";
            if (bricks != null)
                bricks.ForEach(configuration => res += $"\n{configuration}");
            
            return res;
        }
    }
}