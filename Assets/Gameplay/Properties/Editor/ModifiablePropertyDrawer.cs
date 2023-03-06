using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Modifiable))]
public class ModifiablePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Application.isPlaying)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("m_BaseValue"), label, true);
        }
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (Application.isPlaying)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_BaseValue"), label);
        }
    }
}
