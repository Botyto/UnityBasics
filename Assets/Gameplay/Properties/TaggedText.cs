using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TaggedText
{
    private const char TagOpener = '<';
    private const char TagCloser = '>';
    private const char TagNester = '.';
    private const char FunctionArgsOpener = '(';
    private const char FunctionArgsCloser = ')';
    private const char FunctionArgsSeparator = ',';

    public string Text;
    public object Context;

    public TaggedText(string text)
    {
        Text = text;
        Context = null;
    }

    public TaggedText(string text, object context)
    {
        Text = text;
        Context = context;
    }

    public string Process()
    {
        if (Text == null) { return string.Empty; }
        return ProcessTagsInText(Text, Context);
    }

    public override string ToString()
    {
        return Process();
    }

    #region Tag processing

    private static string ProcessTagsInText(string text, object context)
    {
        var sb = new StringBuilder();

        int offset = 0;
        int tagStart, tagEnd;
        while (FindNextTag(text, offset, out tagStart, out tagEnd))
        {
            var before = text.Substring(offset, tagStart - offset);
            var fullTag = text.Substring(tagStart, tagEnd + 1 - tagStart);
            var tag = fullTag.Substring(1, fullTag.Length - 2);
            var tagValue = ProcessExpression(tag, context) ?? fullTag;
            offset = tagEnd + 1;

            sb.Append(before);
            sb.Append(tagValue.ToString());
        }

        if (offset < text.Length)
        {
            var toEnd = text.Substring(offset);
            sb.Append(toEnd);
        }

        return sb.ToString();
    }

    private static object ProcessExpression(string tag, object context)
    {
        var parts = ParseNestedTag(tag);
        if (parts != null && parts.Length > 1)
        {
            object value = null;
            foreach (var part in parts)
            {
                value = ProcessSimpleExpression(part, context);
                context = value;
            }

            return value;
        }

        return ProcessSimpleExpression(tag, context);
    }

    private static object ProcessSimpleExpression(string tag, object context)
    {
        try
        {
            if (ParseFunction(tag, out string funcName, out string[] funcArgs))
            {
                var funcResult = ProcessFunction(funcName, funcArgs, context);
                if (funcResult != null)
                {
                    return funcResult;
                }
            }

            if (ParseComposite(tag, out string tagName, out string[] tagArgs))
            {
                var compositeResult = ProcessComposite(tagName, tagArgs, context);
                if (compositeResult != null)
                {
                    return compositeResult;
                }
            }

            //Property tags
            {
                var propertyResult = ProcessProperty(tag, context);
                if (propertyResult != null)
                {
                    return propertyResult;
                }
            }

            //Literal tags
            {
                var literalResult = ProcessLiteral(tag);
                if (literalResult != null)
                {
                    return literalResult;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Failed to resolve tag '{0}': ", tag, e.Message);
        }

        return null;
    }

    private static object ProcessFunction(string name, string[] args, object context)
    {
        var nameResult = ProcessExpression(name, context);
        var argValues = args.Select(arg => ProcessExpression(arg, context)).ToArray();
        return InvokeFunction(nameResult, argValues, context);
    }

    private static object InvokeFunction(object funcObj, object[] args, object context)
    {
        var target = context;
        if (funcObj is Delegate @delegate)
        {
            funcObj = @delegate.Method;
            target = @delegate.Target;
        }

        if (funcObj is MethodInfo method)
        {
            //TODO check params
            var parameters = method.GetParameters();
            if (args.Length == parameters.Length)
            {
                var allArgsMatch = true;
                for (var i = 0; i < parameters.Length; ++i)
                {
                    if (!parameters[i].ParameterType.IsAssignableFrom(args[i].GetType()))
                    {
                        //Try convert to string as fallback
                        if (parameters[i].ParameterType.IsAssignableFrom(typeof(string)))
                        {
                            if (args[i] != null)
                            {
                                args[i] = args[i].ToString();
                            }
                        }
                        else
                        {
                            allArgsMatch = false;
                            break;
                        }
                    }
                }

                if (allArgsMatch)
                {
                    if (method.IsStatic)
                    {
                        return method.Invoke(null, args);
                    }
                    else if (target != null && method.DeclaringType.IsAssignableFrom(target.GetType()))
                    {
                        return method.Invoke(target, args);
                    }
                }
            }
        }

        return null;
    }

    private static object ProcessComposite(string name, string[] args, object context)
    {
        return ProcessFunction(name, args, context);
    }

    private static object ProcessProperty(string property, object context)
    {
        if (context != null)
        {
            var cSharpValue = ResolveValue(property, context.GetType(), context);
            if (cSharpValue != null)
            {
                return cSharpValue;
            }

            if (context is Component contextComponent)
            {
                var component = contextComponent.GetComponent(property);
                if (component != null)
                {
                    return component;
                }
            }

            if (context is GameObject contextGameObject)
            {
                var component = contextGameObject.GetComponent(property);
                if (component != null)
                {
                    return component;
                }
            }
        }

        return ResolveValue(property, typeof(TaggedTextFuncs), null);
    }

    private static object ResolveValue(string property, Type type, object context)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
        if (context == null)
        {
            bindingFlags |= BindingFlags.Static;
        }
        else
        {
            bindingFlags |= BindingFlags.Instance;
        }

        var fieldInfo = type.GetField(property, bindingFlags);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(context);
        }

        var propertyInfo = type.GetProperty(property, bindingFlags);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(context);
        }

        var methodInfo = type.GetMethod(property, bindingFlags);
        if (methodInfo != null)
        {
            return methodInfo;
        }

        return null;
    }

    private static object ProcessLiteral(string literal)
    {
        if (literal.StartsWith("'") && literal.EndsWith("'")) //'text'
        {
            return literal.Substring(1, literal.Length - 2);
        }
        else if (literal.StartsWith("\"") && literal.EndsWith("\"")) //"text"
        {
            return literal.Substring(1, literal.Length - 2);
        }
        else if (int.TryParse(literal, out int intLiteral)) //1234
        {
            return intLiteral;
        }
        else if (float.TryParse(literal, out float floatLiteral)) //1234.5678
        {
            return floatLiteral;
        }
        else if (bool.TryParse(literal, out bool boolLiteral)) //true false
        {
            return boolLiteral;
        }

        return null;
    }

    #endregion

    #region Tag parsing

    private static string[] ParseNestedTag(string tag)
    {
        return ParseTagParts(tag, TagNester);
    }

    private static bool ParseFunction(string tag, out string name, out string[] args)
    {
        name = null;
        args = null;

        var openIdx = tag.IndexOf(FunctionArgsOpener);
        if (openIdx == -1) { return false; }

        var closeIdx = tag.LastIndexOf(FunctionArgsCloser);
        if (closeIdx == -1) { return false; }

        name = tag.Substring(0, openIdx);
        var argsStr = tag.Substring(openIdx + 1, closeIdx - 1 - openIdx);
        args = ParseFunctionArgs(argsStr);

        return true;
    }

    private static string[] ParseFunctionArgs(string args)
    {
        return ParseTagParts(args, FunctionArgsSeparator);
    }

    private static bool ParseComposite(string tag, out string name, out string[] args)
    {
        name = null;
        args = null;

        var parts = tag.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
        {
            name = parts[0];
            args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);
            return true;
        }

        return false;
    }

    private static bool FindNextTag(string t, int offset, out int tagStart, out int tagEnd)
    {
        tagStart = -1;
        tagEnd = -1;

        var openIdx = t.IndexOf(TagOpener, offset);
        if (openIdx == -1) { return false; }

        var closeIdx = t.IndexOf(TagCloser, openIdx);
        if (closeIdx == -1) { return false; }

        //empty tag
        if (closeIdx == openIdx + 1) { return false; }

        tagStart = openIdx;
        tagEnd = closeIdx;

        return true;
    }

    private static string[] ParseTagParts(string args, char separator)
    {
        var result = new List<string>();

        var startIdx = 0;
        var braketsLevel = 0;
        for (var i = 0; i < args.Length; ++i)
        {
            var c = args[i];
            if (c == FunctionArgsOpener) { ++braketsLevel; continue; }
            if (c == FunctionArgsCloser) { --braketsLevel; continue; }
            if (c == separator && braketsLevel == 0)
            {
                var arg = args.Substring(startIdx, i - startIdx);
                if (string.IsNullOrEmpty(arg))
                {
                    //TODO Debug.LogError();
                }
                else
                {
                    result.Add(arg);
                }

                startIdx = i + 1;
            }
        }

        if (startIdx != args.Length)
        {
            var lastArg = args.Substring(startIdx);
            result.Add(lastArg);
        }

        return result.ToArray();
    }

    #endregion
}
