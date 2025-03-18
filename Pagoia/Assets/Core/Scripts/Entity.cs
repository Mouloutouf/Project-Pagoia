using UnityEngine;

public enum EntityType
{
    Dwarf = 1 << 0,
    Monster = 1 << 1,
    OreBlock = 1 << 2,
    OreCrystal = 1 << 3,
    Pickaxe = 1 << 4,
    CraftingTable = 1 << 5
}

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
