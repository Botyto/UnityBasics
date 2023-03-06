using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRefAttribute : PropertyAttribute
{
    public enum ERefLocation
    {
        This,
        Parent,
        Child,
    }

    public ERefLocation RefLocation;
    public bool Optional;
    public bool IncludeInactive;

    public SceneRefAttribute(ERefLocation refLocation, bool optional = false, bool includeInactive = false)
    {
        RefLocation = refLocation;
        Optional = optional;
        IncludeInactive = includeInactive;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class FindInThisAttribute : SceneRefAttribute
{
    public FindInThisAttribute(bool optional = false, bool includeInactive = false)
        : base(ERefLocation.This, optional, includeInactive)
    { }
}

[AttributeUsage(AttributeTargets.Field)]
public class FindInParentAttribute : SceneRefAttribute
{
    public FindInParentAttribute(bool optional = false, bool includeInactive = false)
        : base(ERefLocation.Parent, optional, includeInactive)
    { }
}

[AttributeUsage(AttributeTargets.Field)]
public class FindInChildAttribute : SceneRefAttribute
{
    public FindInChildAttribute(bool optional = false, bool includeInactive = false)
        : base(ERefLocation.Child, optional, includeInactive)
    { }
}
