using System;

// TODO Can we make this a struct ?
[Serializable]
public class EntityDefinition
{
    public string entityId;
    
    public EntityType entityType;
    
    // TODO Implement the string parsing system to retrieve the min and max values from the entity id
    public int MinQuantity { get; private set; } = 1;
    public int MaxQuantity  { get; private set; } = 0;

    // TODO This absolutely needs to work so check how you should compare both the type and the id to make sure two separate definitions are actually equal
    public override bool Equals(object obj)
    {
        return obj is EntityDefinition other &&
               entityType == other.entityType &&
               entityId == other.entityId;
    }
    
    public static bool operator ==(EntityDefinition obj1, object obj2)
    {
        return obj1 != null && obj1.Equals(obj2);
    }

    public static bool operator !=(EntityDefinition obj1, object obj2)
    {
        return obj1 != null && !obj1.Equals(obj2);
    }
}