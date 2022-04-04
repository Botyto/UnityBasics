using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfPropertyDrawer : ConditionPropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var shouldEnable = CheckCondition(property);
        if (!shouldEnable)
        {
            GUI.enabled = false;
        }

        EditorGUI.PropertyField(position, property, label, true);

        if (!shouldEnable)
        {
            GUI.enabled = true;
        }
    }
}
