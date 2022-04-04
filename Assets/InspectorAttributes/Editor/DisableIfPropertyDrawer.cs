using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisableIfAttribute))]
public class DisableIfPropertyDrawer : ConditionPropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var shouldDisable = CheckCondition(property);
        if (shouldDisable)
        {
            GUI.enabled = false;
        }

        EditorGUI.PropertyField(position, property, label, true);

        if (!shouldDisable)
        {
            GUI.enabled = true;
        }
    }
}
