using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfPropertyDrawer : ConditionPropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return CheckCondition(property) ? 0 : base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!CheckCondition(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
