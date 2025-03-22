using System;
using System.Collections.Generic;
using UnityEngine;

// TODO To Remove
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
        max = _max;
        count = _value;
    }
}

public class Inventory : MonoBehaviour
{
    private Dictionary<EntityType, int> elements = new Dictionary<EntityType, int>();
    private Dictionary<EntityType, int> maximums = new Dictionary<EntityType, int>();

    private void Start()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        maximums = new Dictionary<EntityType, int>()
        {
            { EntityType.Pickaxe, 1 },
            { EntityType.Crystal, 20 }
        };
    }
    
    public void AddElement(Entity _entity)
    {
        if (_entity as Resource == false || _entity as Item == false)
            return;
        
        if (elements.TryGetValue(_entity.entityType, out int count) && count == maximums[_entity.entityType])
            return;
        
        elements[_entity.entityType]++;
    }
    public void RemoveElement(Entity _entity)
    {
        if (elements.ContainsKey(_entity.entityType) == true)
            elements[_entity.entityType]--;
    }
    public int GetElementCount(EntityType _entityType)
    {
        elements.TryGetValue(_entityType, out int count);
        return count;
    }
}
