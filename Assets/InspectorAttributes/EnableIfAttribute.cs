using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class EnableIfAttribute : ConditionAttribute
{
    public EnableIfAttribute(string comparedPropertyName)
        : base(comparedPropertyName)
    { }

    public EnableIfAttribute(string comparedPropertyName, object comparedValue, EComparisonType comparisonType = EComparisonType.Equals)
        : base(comparedPropertyName, comparedValue, comparisonType)
    { }
}
