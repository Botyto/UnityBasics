using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfPropertyDrawer : ConditionPropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return CheckCondition(property) ? base.GetPropertyHeight(property, label) : 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (CheckCondition(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
