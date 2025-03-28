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

    // TODO Goal 2 / Implement the Entity Comparison System
    // TODO First, compare the entity types, using the actual polymorphic types to do the comparison, as such you should make sure we can get the type of an entity from its entity type
    // TODO Second, compare the min and max quantities to make sure they are compatible
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