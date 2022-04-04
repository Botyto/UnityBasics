using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private void OnSubmit(string code)
    {
        InputField.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(code)) { return; }

        print(code);
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
            placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

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
            textText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            //references
            inputField.targetGraphic = inputImage;
            inputField.placeholder = placeholderText;
            inputField.textComponent = textText;

            InputField = inputField;
        }
    }
}
