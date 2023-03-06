using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnityEngine.Object), true)]
public class SceneRefAttributeValidator : Editor
{
    private bool propertyChangedInInspector;

    private void OnEnable()
    {
        Undo.undoRedoPerformed += UpdateAllRefs;
        EditorApplication.hierarchyChanged += UpdateAllRefs;
        AssemblyReloadEvents.afterAssemblyReload += UpdateAllRefs;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= UpdateAllRefs;
        EditorApplication.hierarchyChanged -= UpdateAllRefs;
        AssemblyReloadEvents.afterAssemblyReload -= UpdateAllRefs;
    }

    internal struct FieldAttributePair
    {
        public SceneRefAttribute Attribute;
        public FieldInfo FieldInfo;
    }

    private static Dictionary<Type, List<FieldAttributePair>> FieldAttributesCache = new();
    private static List<FieldAttributePair> GetFieldsWithAttributeFromType(Type cls)
    {
        List<FieldAttributePair> result;
        if (!FieldAttributesCache.TryGetValue(cls, out result))
        {
            result = new List<FieldAttributePair>();
            foreach (var fieldInfo in cls.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                var attributes = fieldInfo.GetCustomAttributes<SceneRefAttribute>();
                foreach (var attribute in attributes)
                {
                    result.Add(new FieldAttributePair
                    {
                        Attribute = attribute,
                        FieldInfo = fieldInfo
                    });
                }
            }
            FieldAttributesCache.Add(cls, result);
        }

        return result;
    }

    private static void UpdateAllRefs()
    {
        foreach (var gameObject in FindObjectsOfType<GameObject>())
        {
            foreach (var component in gameObject.GetComponents<Component>())
            {
                UpdateComponentRefs(component);
            }
        }
    }

    private static void UpdateComponentRefs(Component c)
    {
        var pairs = GetFieldsWithAttributeFromType(c.GetType());
        if (pairs.Count == 0)
        {
            return;
        }

        bool isUninstantiatedPrefab = (c.gameObject.scene.rootCount == 0);
        foreach (var fieldAttributePair in pairs)
        {
            var attribute = fieldAttributePair.Attribute;
            var field = fieldAttributePair.FieldInfo;
            
            object fieldValue = field.GetValue(c);
            if (!Application.isPlaying)
            {
                fieldValue = UpdateRef(attribute, c, field, fieldValue);
            }

            if (isUninstantiatedPrefab)
            {
                continue;
            }

            ValidateRef(attribute, c, field, fieldValue);
        }
    }

    private static object UpdateRef(SceneRefAttribute attr, Component c, FieldInfo field, object existingValue)
    {
        Type fieldType = field.FieldType;
        
        bool isArray = fieldType.IsArray;
        bool includeInactive = attr.IncludeInactive;

        Type elementType = fieldType;
        if (isArray)
        {
            elementType = fieldType.GetElementType();
        }

        object value;
        switch (attr.RefLocation)
        {
            case SceneRefAttribute.ERefLocation.This:
                value = isArray
                    ? c.GetComponents(elementType)
                    : c.GetComponent(elementType);
                break;
            case SceneRefAttribute.ERefLocation.Parent:
                value = isArray
                    ? c.GetComponentsInParent(elementType, includeInactive)
                    : c.GetComponentInParent(elementType, includeInactive);
                break;
            case SceneRefAttribute.ERefLocation.Child:
                value = isArray
                    ? c.GetComponentsInChildren(elementType, includeInactive)
                    : c.GetComponentInChildren(elementType, includeInactive);
                break;
            default:
                throw new Exception($"Unknown Location={attr.RefLocation}");
        }

        if (value == null)
        {
            return existingValue;
        }

        if (isArray)
        {
            if (elementType == null)
            {
                throw new InvalidOperationException();
            }

            Array arrayValue = (Array)value;
            Array resultArray = Array.CreateInstance(elementType, arrayValue.Length);
            Array.Copy(arrayValue, resultArray, resultArray.Length);
            value = resultArray;
        }

        if (existingValue != null && value.Equals(existingValue))
        {
            return existingValue;
        }
        
        field.SetValue(c, value);
        EditorUtility.SetDirty(c);
        
        return value;
    }
    
    private static void ValidateRef(SceneRefAttribute attr, Component c, FieldInfo field, object value)
    {
        Type fieldType = field.FieldType;
        bool isArray = fieldType.IsArray;
        
        if (value == null || value.Equals(null) || (isArray && ((Array)value).Length == 0))
        {
            if (!attr.Optional)
            {
                Debug.LogError($"{c.GetType().Name} missing required {fieldType.Name} ref '{field.Name}'", c.gameObject);
            }
            return;
        }

        if (isArray)
        {
            Array a = (Array)value;
            for (int i = 0; i < a.Length; i++)
            {
                object o = a.GetValue(i);
                ValidateRefLocation(attr.RefLocation, c, field, (Component)o);
            }
        }
        else
        {
            ValidateRefLocation(attr.RefLocation, c, field, (Component)value);
        }
    }

    private static void ValidateRefLocation(SceneRefAttribute.ERefLocation loc, Component c, FieldInfo field, Component refObj)
    {
        switch (loc)
        {
            case SceneRefAttribute.ERefLocation.This:
                if (refObj.gameObject != c.gameObject)
                {
                    Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be on This", c.gameObject);
                }
                break;
            case SceneRefAttribute.ERefLocation.Parent:
                if (!c.transform.IsChildOf(refObj.transform))
                {
                    Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Parent", c.gameObject);
                }
                break;
            case SceneRefAttribute.ERefLocation.Child:
                if (!refObj.transform.IsChildOf(c.transform))
                {
                    Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Child", c.gameObject);
                }
                break;
            default:
                throw new Exception($"Unhandled Location={loc}");
        }
    }
}
