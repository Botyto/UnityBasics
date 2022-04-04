using System.Collections.Generic;
using UnityEngine;

public abstract class UIDialog : UINode
{
    public virtual void Close()
    {
        Destroy(gameObject);
    }

    #region Global dialog management

    public static List<UIDialog> AllDialogs;

    static UIDialog()
    {
        AllDialogs = new List<UIDialog>();
    }

    public static T Get<T>(Transform element)
        where T : UIDialog
    {
        if (element == null) { return null; }

        var dialogComp = element.GetComponent<T>();
        if (dialogComp != null)
        {
            return dialogComp;
        }

        return Get<T>(element.parent);
    }

    public static T Get<T>()
        where T : UIDialog
    {
        var dialogName = DialogName<T>();
        foreach (var dialog in AllDialogs)
        {
            if (dialog is T tDialog && tDialog.name == dialogName)
            {
                return tDialog;
            }
        }

        return null;
    }

    public static T Open<T>(Transform parent)
        where T : UIDialog
    {
        var dialog = Get<T>();
        if (dialog != null) { return dialog; }

        if (parent == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                parent = canvas.transform;
            }
            else
            {
                parent = null;
            }
        }

        var dialogName = DialogName<T>();
        var resource = Resources.Load<GameObject>("Dialogs/" + dialogName);
        if (resource != null)
        {
            var dialogObj = Instantiate(resource, parent);
            var dialogComp = dialogObj.GetComponent<T>();
            if (dialogComp == null)
            {
                Debug.LogErrorFormat("Dialog resource '{0}' has no Dialog component of that type!", dialogName);
            }

            return dialogComp;
        }
        else
        {
            var dialogObj = new GameObject(dialogName);
            dialogObj.transform.parent = parent;
            var dialogComp = dialogObj.AddComponent<T>();
            return dialogComp;
        }
    }

    public static void Close<T>()
        where T : UIDialog
    {
        var dialog = Get<T>();
        if (dialog != null)
        {
            dialog.Close();
        }
    }

    #endregion

    #region Internals

    private static string DialogName<T>()
    {
        return typeof(T).Name;
    }

    protected virtual void Start()
    {
        SendMessage("OnOpenDialog", SendMessageOptions.DontRequireReceiver);
    }

    protected virtual void OnDestroy()
    {
        SendMessage("OnCloseDialog", SendMessageOptions.DontRequireReceiver);
    }

    #endregion
}
