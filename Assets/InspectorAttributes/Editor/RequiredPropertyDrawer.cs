using UnityEditor;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredPropertyDrawer : InfoBoxPropertyDrawer
{
    protected override bool ShouldShowInfoBox(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
            case SerializedPropertyType.AnimationCurve:
            case SerializedPropertyType.Gradient:
                return property.objectReferenceValue == null;
            case SerializedPropertyType.ExposedReference:
                return property.exposedReferenceValue == null;
            case SerializedPropertyType.ManagedReference:
                return property.managedReferenceValue == null;
            default: return false;
        }
    }
}
