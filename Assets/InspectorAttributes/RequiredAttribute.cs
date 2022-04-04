using System;
using UnityEditor;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class RequiredAttribute : InfoBoxAttribute
{
    public RequiredAttribute(string message = null, MessageType messageType = MessageType.Error)
        : base(message, messageType)
    { }
}
