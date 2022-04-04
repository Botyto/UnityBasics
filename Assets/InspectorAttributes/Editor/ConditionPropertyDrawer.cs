using System;
using UnityEditor;

public abstract class ConditionPropertyDrawer : PropertyDrawer
{
    protected bool CheckCondition(SerializedProperty property)
    {
        var conditionAttr = attribute as ConditionAttribute;
        var comparedProperty = property.serializedObject.FindProperty(conditionAttr.PropertyName);
        if (comparedProperty != null)
        {
            return ComparePropertyValue(comparedProperty, conditionAttr.Value, conditionAttr.Comparison);
        }
        var method = property.serializedObject.targetObject.GetType().GetMethod(conditionAttr.PropertyName);
        if (method != null)
        {
            var result = method.Invoke(property.serializedObject.targetObject, new object[0]);
            return CompareValue(result, conditionAttr.Value, conditionAttr.Comparison);
        }
        return false;
    }

    #region Value comparison

    private static bool ComparePropertyValue(SerializedProperty lhs, object rhs, ConditionAttribute.EComparisonType comparison)
    {
        object lhsValue;
        switch (lhs.propertyType)
        {
            case SerializedPropertyType.Integer: lhsValue = lhs.intValue; break;
            case SerializedPropertyType.Boolean: lhsValue = lhs.boolValue; break;
            case SerializedPropertyType.Float: lhsValue = lhs.floatValue; break;
            case SerializedPropertyType.String: lhsValue = lhs.stringValue; break;
            case SerializedPropertyType.Color: lhsValue = lhs.colorValue; break;
            case SerializedPropertyType.ObjectReference: lhsValue = lhs.objectReferenceValue; break;
            case SerializedPropertyType.LayerMask: lhsValue = lhs.intValue; break;
            case SerializedPropertyType.Enum: lhsValue = lhs.enumValueFlag; break;
            case SerializedPropertyType.Vector2: lhsValue = lhs.vector2Value; break;
            case SerializedPropertyType.Vector3: lhsValue = lhs.vector3Value; break;
            case SerializedPropertyType.Vector4: lhsValue = lhs.vector4Value; break;
            case SerializedPropertyType.Rect: lhsValue = lhs.rectValue; break;
            case SerializedPropertyType.ArraySize: lhsValue = lhs.arraySize; break;
            case SerializedPropertyType.Character: lhsValue = lhs.stringValue; break;
            case SerializedPropertyType.AnimationCurve: lhsValue = lhs.animationCurveValue; break;
            case SerializedPropertyType.Bounds: lhsValue = lhs.boundsValue; break;
            case SerializedPropertyType.Gradient: lhsValue = lhs.animationCurveValue; break;
            case SerializedPropertyType.Quaternion: lhsValue = lhs.quaternionValue; break;
            case SerializedPropertyType.ExposedReference: lhsValue = lhs.exposedReferenceValue; break;
            case SerializedPropertyType.FixedBufferSize: lhsValue = lhs.fixedBufferSize; break;
            case SerializedPropertyType.Vector2Int: lhsValue = lhs.vector2IntValue; break;
            case SerializedPropertyType.Vector3Int: lhsValue = lhs.vector3IntValue; break;
            case SerializedPropertyType.RectInt: lhsValue = lhs.rectIntValue; break;
            case SerializedPropertyType.BoundsInt: lhsValue = lhs.boundsIntValue; break;
            case SerializedPropertyType.ManagedReference: lhsValue = lhs.managedReferenceValue; break;
            case SerializedPropertyType.Hash128: lhsValue = lhs.hash128Value; break;
            default: return false;
        }

        return CompareValue(lhsValue, rhs, comparison);
    }

    private static bool CompareValue(object lhs, object rhs, ConditionAttribute.EComparisonType comparison)
    {
        switch (comparison)
        {
            case ConditionAttribute.EComparisonType.Equals: return Equals(lhs, rhs);
            case ConditionAttribute.EComparisonType.NotEqual: return !Equals(lhs, rhs);
            default: return CompareNumericValue(lhs, rhs, comparison);
        }
    }

    private static bool CompareNumericValue(object lhs, object rhs, ConditionAttribute.EComparisonType comparison)
    {
        var comparableLhs = (IComparable)lhs;
        var result = comparableLhs.CompareTo(rhs);
        switch (comparison)
        {
            case ConditionAttribute.EComparisonType.Equals: return result == 0;
            case ConditionAttribute.EComparisonType.NotEqual: return result != 0;
            case ConditionAttribute.EComparisonType.GreaterThan: return result > 0;
            case ConditionAttribute.EComparisonType.LesserThan: return result < 0;
            case ConditionAttribute.EComparisonType.GreaterOrEqual: return result >= 0;
            case ConditionAttribute.EComparisonType.LesserOrEqual: return result <= 0;
            default: return false;
        }
    }

    #endregion
}
