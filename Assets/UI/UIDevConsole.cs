/*
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;

#if UNITY_EDITOR
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
#endif

public class UIDevConsole : UIDialog
{
    public Text LogText;
    public InputField InputField;

    [Header("Action keys")]
    public KeyCode OpenConsoleKey = KeyCode.BackQuote;
    public KeyCode ClearLogKey = KeyCode.F9;

    private void OnOpenDialog()
    {
        InitUI();
        InputField.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(OpenConsoleKey))
        {
            ToggleConsole();
        }
        else if (Input.GetKeyDown(ClearLogKey))
        {
            ClearLog();
        }
    }

    public void ToggleConsole()
    {
        if (!InputField.gameObject.activeSelf)
        {
            InputField.gameObject.SetActive(true);
            InputField.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(InputField.gameObject);
        }
        else
        {
            InputField.gameObject.SetActive(false);
        }
    }

    public void ClearLog()
    {
        if (LogText != null)
        {
            LogText.text = string.Empty;
        }
    }

    static readonly string[] CodeUsingItems = new string[] { "System", "UnityEngine", "UnityEditor" };
    static readonly string CodeUsings = string.Join(" ", CodeUsingItems.Select(x => $"using {x};")) + " ";
    static readonly string[] CodeHelperFuncs = new string[] {
    };
    static readonly string[] CodeBuiltInHelperFuncs = new string[] {
        "UnityEngine.Object.Instantiate, UnityEngine",
        "UnityEngine.Object.Destroy, UnityEngine",
        "UnityEngine.Object.DestroyImmediate, UnityEngine",
        "UnityEngine.Object.FindObjectsOfType, UnityEngine",
        "UnityEngine.Object.FindSceneObjectsOfType, UnityEngine",
        "UnityEngine.MonoBehaviour.print, UnityEngine"
    };
    const string CodeClassName = "DbgExec";
    const string CodeFuncName = "Cmd";
    const string CodeClassDef = "public class "+CodeClassName+" {{\n {0}\n {1}\n {2}\n }}";
    static string[] CodeFormatsCache = null;
    static string[] CodeFuncFormats = new string[] {
        "public void "+CodeFuncName+"() { print({0}); }",
        "public void "+CodeFuncName+"() { {0}; }",
    };

    private void OnSubmit(string code)
    {
        InputField.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(code)) { return; }

#if UNITY_EDITOR
        if (CodeFormatsCache == null)
        {
            string builtInHelperFuncs = "";
            foreach (string path in CodeBuiltInHelperFuncs)
            {
                var dotIdx = path.LastIndexOf(".");
                var classPath = path.Substring(0, dotIdx);
                var typePath = classPath;
                var funcName = path.Substring(dotIdx+1);
                var commaIdx = path.IndexOf(",");
                if (commaIdx > 0) {
                    typePath += ", " + funcName.Substring(commaIdx - dotIdx);
                    funcName = funcName.Substring(0, commaIdx - dotIdx - 1);
                }
                var classType = Type.GetType(typePath);
                foreach (var methodInfo in classType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                {
                    if (methodInfo.Name != funcName) { continue; }
                    if (methodInfo.GetCustomAttribute<ObsoleteAttribute>() != null) { continue; }
                    string funcDef = $"public static ";
                    if (methodInfo.ReturnType == typeof(void)) { funcDef += "void"; }
                    else { funcDef += methodInfo.ReturnType.ToString(); };
                    funcDef += " " + funcName;
                    if (methodInfo.IsGenericMethod)
                    {
                        funcDef += "<";
                        foreach (var genericType in methodInfo.GetGenericArguments())
                        {
                            funcDef += genericType.Name;
                        }
                        funcDef += ">";
                    }
                    funcDef += "(";
                    funcDef += string.Join(", ", methodInfo.GetParameters().Select(pi => $"{pi.ParameterType} {pi.Name}"));
                    funcDef += ") { ";
                    if (methodInfo.ReturnType != typeof(void)) { funcDef += "return "; }
                    funcDef += $"{classPath}.{funcName}";
                    if (methodInfo.IsGenericMethod)
                    {
                        funcDef += "<";
                        foreach (var genericType in methodInfo.GetGenericArguments())
                        {
                            funcDef += genericType.Name;
                        }
                        funcDef += ">";
                    }
                    funcDef += "(";
                    funcDef += string.Join(", ", methodInfo.GetParameters().Select(pi => pi.Name));
                    funcDef += "); }";
                    builtInHelperFuncs += $"{funcDef}\n";
                }
            }
            CodeFormatsCache = CodeFuncFormats.Select(fn =>
                string.Format(
                    $"{CodeUsings}\n{CodeClassDef}",
                    string.Join("\n", CodeHelperFuncs.Select(fn => $"\t{fn}")),
                    builtInHelperFuncs,
                    fn)
                .Replace("{0}", "!@#$")
                .Replace("{", "{{")
                .Replace("}", "}}")
                .Replace("!@#$", "{0}")
                ).ToArray();
        }
        foreach (string format in CodeFormatsCache)
        {
            if (TryExecuteCSharp(format, code))
            {
                break;
            }
        }
#else
        print("Console is only available in the Unity editor");
#endif
    }

    private bool TryExecuteCSharp(string format, string code)
    {
        var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters(new string[] { "UnityEngine" });
        parameters.GenerateInMemory = true;
        parameters.GenerateExecutable = false;
        parameters.IncludeDebugInformation = false;
        var fullCode = string.Format(format, code);
        var results = provider.CompileAssemblyFromSource(parameters, fullCode);
        if (results.Errors.HasErrors)
        {
            for (int i = 0; i < results.Errors.Count; ++i)
            {
                Debug.LogError(results.Errors[i]);
            }
            return false;
        }

        var assembly = results.CompiledAssembly;
        var instance = assembly.CreateInstance(CodeClassName);
        var methodInfo = instance.GetType().GetMethod(CodeFuncName);
        try
        {
            methodInfo.Invoke(instance, null);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }

        return true;
    }

    private void InitUI()
    {
        //Self
        {
            var myTransform = transform as RectTransform;
            myTransform.anchorMin = Vector2.zero;
            myTransform.anchorMax = Vector2.one;
            myTransform.offsetMin = Vector2.zero;
            myTransform.offsetMax = Vector2.zero;
        }

        //Log
        {
            var logObj = new GameObject("Log");
            logObj.transform.parent = transform;
            var logTransform = logObj.AddComponent<RectTransform>();
            logTransform.anchorMin = Vector2.zero;
            logTransform.anchorMax = Vector2.one;
            logTransform.offsetMin = new Vector2(0, 30);
            logTransform.offsetMax = new Vector2(0, 0);
            var logText = logObj.AddComponent<Text>();
            logText.alignment = TextAnchor.LowerLeft;

            LogText = logText;
        }

        //Input field
        {
            var inputObj = new GameObject("Input");
            inputObj.transform.parent = transform;
            var inputTransform = inputObj.AddComponent<RectTransform>();
            inputTransform.pivot = new Vector2(0.5f, 0);
            inputTransform.anchorMin = new Vector2(0, 0);
            inputTransform.anchorMax = new Vector2(1, 0);
            inputTransform.offsetMin = new Vector2(0, 0);
            inputTransform.offsetMax = new Vector2(0, 30);
            inputObj.AddComponent<CanvasRenderer>();
            var inputImage = inputObj.AddComponent<Image>();
            inputImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
            inputImage.type = Image.Type.Sliced;
            var inputField = inputObj.AddComponent<InputField>();
            inputField.onEndEdit.AddListener(OnSubmit);

            var placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.parent = inputObj.transform;
            var placeholderTransform = placeholderObj.AddComponent<RectTransform>();
            placeholderTransform.anchorMin = Vector2.zero;
            placeholderTransform.anchorMax = Vector2.one;
            placeholderTransform.offsetMin = new Vector2(10, 6);
            placeholderTransform.offsetMax = new Vector2(-10, -7);
            var placeholderText = placeholderObj.AddComponent<Text>();
            placeholderText.fontStyle = FontStyle.Italic;
            placeholderText.text = "Enter command...";
            placeholderText.color = Color.gray;
            placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var textObj = new GameObject("Text");
            textObj.transform.parent = inputObj.transform;
            var textTransform = textObj.AddComponent<RectTransform>();
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = new Vector2(10, 6);
            textTransform.offsetMax = new Vector2(-10, -7);
            var textText = textObj.AddComponent<Text>();
            textText.supportRichText = false;
            textText.color = Color.black;
            textText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            //references
            inputField.targetGraphic = inputImage;
            inputField.placeholder = placeholderText;
            inputField.textComponent = textText;

            InputField = inputField;
        }
    }
}
*/