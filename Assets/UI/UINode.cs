using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UINode : MonoBehaviour
{
    private static UINode FindParentNode(Transform obj)
    {
        if (obj == null) { return null; }

        var node = obj.GetComponent<UINode>();
        if (node != null)
        {
            return node;
        }

        return FindParentNode(obj.transform.parent);
    }

    public static UINode ResolveNode(Transform obj, string name)
    {
        var node = FindParentNode(obj);
        if (node != null)
        {
            return node.ResolveNode(name);
        }

        return null;
    }

    public static T ResolveNode<T>(Transform obj, string name)
        where T : MonoBehaviour
    {
        var node = FindParentNode(obj);
        if (node != null)
        {
            return node.ResolveNode<T>(name);
        }

        return null;
    }

    public UINode ResolveNode(string name)
    {
        var nodesInChildren = GetComponentsInChildren<UINode>();
        foreach (var node in nodesInChildren)
        {
            if (node.name != name) { continue; }

            return node;
        }

        return null;
    }

    public T ResolveNode<T>(string name)
        where T : MonoBehaviour
    {
        var nodesInChildren = GetComponentsInChildren<UINode>();
        foreach (var node in nodesInChildren)
        {
            if (node.name != name) { continue; }

            var behaviour = node.GetComponent<T>();
            if (behaviour == null) { continue; }

            return behaviour;
        }

        return null;
    }
}
