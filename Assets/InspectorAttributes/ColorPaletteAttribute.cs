using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ColorPaletteAttribute : PropertyAttribute
{
    public string Palette;

    public ColorPaletteAttribute(string palette)
    {
        Palette = palette;
    }
}
