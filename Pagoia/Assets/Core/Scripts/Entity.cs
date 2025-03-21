using UnityEngine;

// We might change this later to use Type directly, but I do not know how to implement it and make it accessible through Unity inspector at the moment
// Ideally, we should make use of Type and create a custom tool attribute in order to display types as a dropdown selection list in the inspector

// TODO Make a dictionary to associate each Entity Type with their C# Type so we can use this information when searching instead of the Entity Type
// Especially useful for generic states that can be satisfied for any kind of entities, and where we need to know that our specific entity type is indeed valid for that state
// which uses another entity type that is above in the inheritance hierarchy
// Essentially the Entity Type is only here to allow the user to input that information easily in the inspector but it should not be used !!!

public enum EntityType
{
    None,
    Entity,
    Pickable,
    Item,
    Resource,
    Collector,
    Block,
    Agent,
    Pickaxe,
    Crystal,
    OreBlock
}

public class Entity : MonoBehaviour
{
    public EntityType entityType;

    public GameObject model;

    protected virtual void Start()
    {
        World.instance.AddState(StatusType.Exists, this);
    }

    protected virtual void OnDisable()
    {
        World.instance.RemoveState(StatusType.Exists, this);
    }
}
