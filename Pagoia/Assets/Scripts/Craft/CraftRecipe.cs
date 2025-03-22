using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Iron, Diamond, Wood }

public enum ItemType { Pickaxe, Sword, Bow, Arrows, Shield }

public class CraftRecipe : ScriptableObject
{
    public ItemType itemToCraft;
    public int outputNumber;

    public Dictionary<ResourceType, int> resourcesRequired;

    public GameObject itemPrefab;
}
