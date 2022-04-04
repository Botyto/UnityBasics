using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ClassNameAttribute : PropertyAttribute
{
    public Type BaseClass;
    public bool IncludeBaseClass;

    public ClassNameAttribute(Type baseClass, bool includeBaseClass = true)
    {
        BaseClass = baseClass;
        IncludeBaseClass = includeBaseClass;
    }
}
