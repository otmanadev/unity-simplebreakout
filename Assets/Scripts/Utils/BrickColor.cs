using System.Collections.Generic;
using UnityEngine;

public static class BrickColor
{
    
    private static List<Color> colors = new List<Color>()
    {
        Color.red,
        Color.green,
        Color.cyan,
        Color.blue,
        Color.magenta,
        Color.yellow,
        new Color(1f, .5f, .0f)
    };

    /// <summary>
    /// Select random color.
    /// </summary>
    /// <returns></returns>
    public static Color PickRandomColor()
    {
        return colors[Random.Range(0, colors.Count)];
    }
    
}