using UnityEngine;

public static class ColorMapper
{
    public static Color GetColor(NutColor nutColor)
    {
        return nutColor switch
        {
            NutColor.Red => Color.red,
            NutColor.Green => Color.green,
            NutColor.Blue => Color.blue,
            NutColor.Yellow => Color.yellow,
            NutColor.CadetBlue => Color.cadetBlue,
            NutColor.Brown => Color.brown,
            NutColor.Orange => Color.orange,
            NutColor.Pink => Color.deepPink,
            _ => Color.white
        };
    }
}