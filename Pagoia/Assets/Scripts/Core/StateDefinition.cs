using System;

// TODO Can we make this a struct ?
[Serializable]
public class StateDefinition
{
    public int priority;
    
    public EntityDefinition firstEntity;

    public StatusType statusType;

    public EntityDefinition secondEntity;

    public override bool Equals(object obj)
    {
        return obj is StateDefinition other &&
               firstEntity == other.firstEntity &&
               statusType == other.statusType &&
               secondEntity == other.secondEntity;
    }

    public static bool operator ==(StateDefinition obj1, object obj2)
    {
        return obj1 != null && obj1.Equals(obj2);
    }

    public static bool operator !=(StateDefinition obj1, object obj2)
    {
        return obj1 != null && !obj1.Equals(obj2);
    }
}
