using System;

public enum Target { Same = 0, New = 1 }

[Serializable]
public class StateTemplate
{
    public StateType stateType;

    [EnumFlags(typeof(EntityType))]
    public EntityType entityType;
    public bool anyEntity;

    public Target targetType;
}
