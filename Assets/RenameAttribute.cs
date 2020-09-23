using UnityEngine;
// Type [Rename] to rename an attribute to an different name

public class RenameAttribute : PropertyAttribute
{
    public string NewName { get; private set; }
    public RenameAttribute(string name)
    {
        NewName = name;
    }
}
