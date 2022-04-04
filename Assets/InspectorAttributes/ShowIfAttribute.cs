using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ShowIfAttribute : ConditionAttribute
{
    public ShowIfAttribute(string comparedPropertyName)
        : base(comparedPropertyName)
    { }

    public ShowIfAttribute(string comparedPropertyName, object comparedValue, EComparisonType comparisonType = EComparisonType.Equals)
        : base(comparedPropertyName, comparedValue, comparisonType)
    { }
}
