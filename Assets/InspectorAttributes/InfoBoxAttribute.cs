using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class InfoBoxAttribute : PropertyAttribute
{
    public string Message;
    public MessageType MessageType;

    public InfoBoxAttribute(string message, MessageType messageType = MessageType.Info)
    {
        Message = message;
        MessageType = messageType;
    }
}
