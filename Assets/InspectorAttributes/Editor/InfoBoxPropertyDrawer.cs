using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InfoBoxAttribute))]
public class InfoBoxPropertyDrawer : PropertyDrawer
{
    private const int c_Margin = 2;

    private InfoBoxAttribute InfoBoxAttribute => attribute as InfoBoxAttribute;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var propHeight = base.GetPropertyHeight(property, label);
        if (ShouldShowInfoBox(property))
        {
            var content = new GUIContent(InfoBoxAttribute.Message);
            var infoBoxHeight = EditorStyles.helpBox.CalcHeight(content, EditorGUIUtility.currentViewWidth);
            return propHeight + Mathf.Max(infoBoxHeight, EditorStyles.helpBox.lineHeight * 3) + c_Margin * 2;
        }
        return propHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (ShouldShowInfoBox(property))
        {
            var helpPos = position;
            var propHeight = base.GetPropertyHeight(property, label);
            helpPos.height -= propHeight + c_Margin;
            EditorGUI.HelpBox(helpPos, InfoBoxAttribute.Message, InfoBoxAttribute.MessageType);
            position.y += helpPos.height + c_Margin;
            position.height = propHeight;
        }
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndProperty();
    }

    protected virtual bool ShouldShowInfoBox(SerializedProperty property)
    {
        return true;
    }
}
