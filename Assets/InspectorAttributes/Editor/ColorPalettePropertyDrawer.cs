using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorPaletteAttribute))]
public class ColorPalettePropertyDrawer : ConditionPropertyDrawer
{
    private static string s_InvalidTypeMessage = "Use ColorPalette with Color.";

    private ColorPaletteAttribute colorPaletteAttribute => attribute as ColorPaletteAttribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (property.propertyType == SerializedPropertyType.Color)
        {
            var colors = GetColors(colorPaletteAttribute.Palette);
            //TODO...
        }
        else
        {
            EditorGUI.LabelField(position, label.text, s_InvalidTypeMessage);
        }
        EditorGUI.EndProperty();
    }

    private Color[] GetColors(string colorPalette)
    {
        return new Color[]
        {
            Color.red,
            Color.green,
            Color.blue,
        };
    }
}
