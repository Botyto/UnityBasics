using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DisableIfAttribute : ConditionAttribute
{
    public DisableIfAttribute(string comparedPropertyName)
        : base(comparedPropertyName)
    { }

    public DisableIfAttribute(string comparedPropertyName, object comparedValue, EComparisonType comparisonType = EComparisonType.Equals)
        : base(comparedPropertyName, comparedValue, comparisonType)
    { }
}
