using System;

[Serializable]
public class NameTag : ICmsComponentDefinition
{
    public string name;
    public override string ToString()
    {
        return name;
    }
}