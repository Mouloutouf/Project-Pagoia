using UnityEngine;

// TODO Remove the flags on the Entity Type as it can and probably will become a very large enum which will not be able to handle flags
// TODO For now, just use the enum to represent every single possible entity in the world, from the most abstract to the least
// We might change this later to use Type directly but I do not know how to implement it and make it accessible through Unity inspector at the moment
// Ideally, we should make use of Type and create a custom tool attribute in order to display types as a dropdown selection list in the inspector

public enum EntityType
{
    Dwarf = 1 << 0,
    Monster = 1 << 1,
    OreBlock = 1 << 2,
    OreCrystal = 1 << 3,
    Pickaxe = 1 << 4,
    CraftingTable = 1 << 5
}

// TODO Make an inheritance hierarchy for entities instead of having it be separate
// It makes more sense to do so, as we need an abstraction hierarchy for the whole states and entities system,
// and there really is no reason not to have inheritance here

public class Entity : MonoBehaviour
{
    public EntityType entityType;

    public GameObject model;

    public void AddEntityAsTarget(StateType _type)
    {
        World.instance.AddState(_type, this);
    }
    public void RemoveEntityAsTarget(StateType _type)
    {
        World.instance.RemoveState(_type, this);
    }

    private void Start() => AddEntityAsTarget(StateType.Exists);
    private void OnDisable() => RemoveEntityAsTarget(StateType.Exists);
}
