using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ClassNameAttribute))]
public class ClassNamePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var className = attribute as ClassNameAttribute;
        var baseType = className.BaseClass;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var displayedTypesList = new List<Type>();
        foreach (var assembly in assemblies)
        {
            var assemblyTypes = assembly.GetTypes();
            foreach (var type in assemblyTypes)
            {
                if (type == baseType && className.IncludeBaseClass || type.IsSubclassOf(baseType))
                {
                    displayedTypesList.Add(type);
                }
            }
        }
        var typeNamesList = displayedTypesList.Select(type => type.Name).ToList();
        var selectedIdx = typeNamesList.IndexOf(property.stringValue);
        if (selectedIdx == -1) { selectedIdx = 0; }
        var displayedOptions = new GUIContent[displayedTypesList.Count];
        for (var i = 0; i < displayedTypesList.Count; ++i) {
            var type = displayedTypesList[i];
            var tooltipAttrib = type.GetCustomAttribute<TooltipAttribute>();
            displayedOptions[i] = new GUIContent(type.Name, tooltipAttrib?.tooltip);
        }
        var newIdx = EditorGUI.Popup(position, label, selectedIdx, displayedOptions);
        property.stringValue = typeNamesList[newIdx];
    }
}