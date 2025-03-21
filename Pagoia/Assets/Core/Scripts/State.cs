public enum StatusType
{
    Exists,
    InRangeOf,
    InPhysicalSpace,
    Inside,
    SpawnedBy,
    DestroyedBy,
    PickedUpBy,
    DroppedBy
}

public class State
{
    public Entity targetEntity;
    
    public StatusType statusType;
    
    public Entity otherEntity;
}
