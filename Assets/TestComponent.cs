using UnityEngine;

public class TestComponent : MonoBehaviour
{
    public Modifiable ModifiableValue;

    public void Start()
    {
        var taggedText = new TaggedText("<objname('TestObject').Transform.position>", this);
        print($"'{taggedText.Text}' -> '{taggedText.Process()}'");
    }

    public string TestMethod(string str)
    {
        print(str);
        return str;
    }
}
