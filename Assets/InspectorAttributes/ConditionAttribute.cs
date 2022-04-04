using UnityEngine;

public class ConditionAttribute : PropertyAttribute
{
    public enum EComparisonType
    {
        Equals,
        NotEqual,
        GreaterThan,
        LesserThan,
        LesserOrEqual,
        GreaterOrEqual,
    }

    public string PropertyName;
    public object Value;
    public EComparisonType Comparison;

    public ConditionAttribute(string comparedPropertyName)
    {
        PropertyName = comparedPropertyName;
        Value = true;
        Comparison = EComparisonType.Equals;
    }

    public ConditionAttribute(string comparedPropertyName, object comparedValue, EComparisonType comparisonType = EComparisonType.Equals)
    {
        PropertyName = comparedPropertyName;
        Value = comparedValue;
        Comparison = comparisonType;
    }
}
