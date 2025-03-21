using System;

[Serializable]
public class EntityDefinition
{
    public string entityId;
    
    public EntityType entityType;
    
    public int minQuantity = 1;
    public int maxQuantity = 0;
}