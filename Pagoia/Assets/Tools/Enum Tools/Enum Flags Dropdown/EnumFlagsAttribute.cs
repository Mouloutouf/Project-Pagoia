using System;
using UnityEngine;

public class EnumFlagsAttribute : PropertyAttribute
{
    public Type type;
    public EnumFlagsAttribute(Type _type)
    {
        this.type = _type;
    }
}
