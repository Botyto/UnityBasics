using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class HideIfAttribute : ConditionAttribute
{
    public HideIfAttribute(string comparedPropertyName)
        : base(comparedPropertyName)
    { }

    public HideIfAttribute(string comparedPropertyName, object comparedValue, EComparisonType comparisonType = EComparisonType.Equals)
        : base(comparedPropertyName, comparedValue, comparisonType)
    { }
}
