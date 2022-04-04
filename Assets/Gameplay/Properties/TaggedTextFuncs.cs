using UnityEngine;

public partial class TaggedTextFuncs
{
    public static GameObject objname(string name)
    {
        return GameObject.Find(name);
    }

    public static GameObject objtag(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }

    public static GameObject objchild(Object unityObject, int childIdx)
    {
        if (unityObject is GameObject gameObject)
        {
            return gameObject.transform.GetChild(childIdx).gameObject;
        }
        else if (unityObject is Transform transform)
        {
            return transform.GetChild(childIdx).gameObject;
        }
        else if (unityObject is Component component)
        {
            return component.transform.GetChild(childIdx).gameObject;
        }
        return null;
    }
}
