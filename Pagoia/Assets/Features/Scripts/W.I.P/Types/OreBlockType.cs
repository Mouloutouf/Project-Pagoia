using UnityEngine;

public interface IEntityType
{

}

public class RessourceType : ScriptableObject, IEntityType { }

public class OreType : RessourceType
{
    public GameObject orePrefab;
}

public class OreBlockType : ScriptableObject, IEntityType
{
    public OreType oreType;

    public GameObject oreBlockPrefab;
}
