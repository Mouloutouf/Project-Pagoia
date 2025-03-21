using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElementCount
{
    public int max;

    private int count;
    public int Count
    {
        get => count;
        set => count = Mathf.Clamp(value, 0, max);
    }

    public ElementCount(int _max, int _value)
    {
        this.max = _max;
        this.count = _value;
    }
}

public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public List<Entity> crystals = new List<Entity>();
    public int crystalCount => crystals.Count;

    [HideInInspector]
    public Entity pickaxe;

    public Dictionary<EntityType, int> maximums = new Dictionary<EntityType, int>();

    private Dictionary<EntityType, ElementCount> storage = new Dictionary<EntityType, ElementCount>();

    public void AddElement(EntityType _element)
    {
        if (storage.ContainsKey(_element))
        {
            storage[_element].Count++;
        }
        else
        {
            storage.Add(_element, new ElementCount(maximums[_element], 1));
        }
    }
    public void RemoveElement(EntityType _element)
    {
        if (storage.ContainsKey(_element))
        {
            storage[_element].Count--;
        }
        else
        {
            Debug.LogError($"There are no elements of type {_element} in the inventory");
        }
    }
}
