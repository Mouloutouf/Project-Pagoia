using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;
    private void Awake()
    {
        instance ??= this;
        states.Clear();

        foreach (var s in states)
        {
            Debug.LogError($"State {s.statusType} with entity {s.targetEntity.entityType}");
        }
    }

    private List<State> states = new List<State>();

    public Entity[] FindEntities(StatusType _statusType, EntityType _entityType)
    {
        var entitySearchQuery =
            from state in states where (state.statusType == _statusType) && (state.targetEntity.entityType == _entityType) select state.targetEntity;

        return entitySearchQuery.ToArray();
    }
    public Entity[] FindEntities(EntityType _entityType, params (StatusType stateType, bool shouldHaveState)[] _statesToConfirm)
    {
        // Start by Querying all the states that are related to that specific entity type
        var entitiesStatesQuery =
            from state in states where state.targetEntity.entityType == _entityType select state;

        // This is costly and quite heavy, but it's the only way to achieve what I want
        List<Entity> validEntities = new List<Entity>();
        List<Entity> invalidEntities = new List<Entity>();
        foreach (var entityState in entitiesStatesQuery)
        {
            foreach (var stateToConfirm in _statesToConfirm)
            {
                var condition = stateToConfirm.shouldHaveState
                    ? entityState.statusType == stateToConfirm.stateType
                    : entityState.statusType != stateToConfirm.stateType;
                
                if (condition)
                {
                    if (invalidEntities.Contains(entityState.targetEntity))
                        continue;
                    if (!validEntities.Contains(entityState.targetEntity))
                        validEntities.Add(entityState.targetEntity);
                }
                else
                {
                    if (!invalidEntities.Contains(entityState.targetEntity))
                        invalidEntities.Add(entityState.targetEntity);
                    if (validEntities.Contains(entityState.targetEntity))
                        validEntities.Remove(entityState.targetEntity);
                }
            }
        }

        return validEntities.ToArray();
        
        // Then for each state we want to look at, re-query the same query, until we get all the entities that confirm all these states
        foreach (var stateToConfirm in _statesToConfirm)
        {
            if (stateToConfirm.shouldHaveState)
            {
                entitiesStatesQuery = 
                    from state in entitiesStatesQuery where state.statusType == stateToConfirm.stateType select state;
            }
            else
            {
                entitiesStatesQuery = 
                    from state in entitiesStatesQuery where state.statusType != stateToConfirm.stateType select state;
            }
            
            // stateQuery = 
            //     from state in stateQuery where stateToConfirm.shouldHaveState ? 
            //         state.stateType == stateToConfirm.stateType : 
            //         state.stateType != stateToConfirm.stateType 
            //     select state;
        }

        // End by querying the entity for each of the queried states
        var entitySearchQuery =
            from state in entitiesStatesQuery select state.targetEntity;

        return entitySearchQuery.ToArray();
    }

    public bool ContainsState(StatusType _statusType, EntityType _entityType)
    {
        var searchedStateQuery =
            from state in states where (state.statusType == _statusType) && (state.targetEntity.entityType == _entityType) select state;
        //foreach (var s in states)
        //{
        //    Debug.LogError($"State {s.stateType} with entity {s.target.entityType}");
        //}
        //Debug.LogError($"State {_stateType} with entity {_entityType} is satisfied ? {searchedStateQuery.Any()}");

        return searchedStateQuery.Any();
    }
    public bool ContainsState(StatusType _statusType, Entity _target, Entity _other = null)
    {
        var stateQuery =
            from state in states where (state.statusType == _statusType) && (state.targetEntity == _target) && (state.otherEntity == _other)
            select state;

        return stateQuery.Any();
    }

    public State GetState(StatusType _statusType, Entity _target, Entity _other = null)
    {
        var stateQuery =
            from state in states where (state.statusType == _statusType) && (state.targetEntity == _target) && (state.otherEntity == _other)
            select state;

        var stateList = stateQuery.ToList();
        if (stateList.Any())
            return stateList.First();
        
        Debug.LogError($"No state found where {_target.entityType} has status {_statusType} towards {(_other != null ? _other.entityType : null)}");
        return null;
    }

    public void AddState(StatusType _statusType, Entity _target, Entity _other = null)
    {
        if (ContainsState(_statusType, _target, _other) == true)
            return;
        
        states.Add(new State { targetEntity = _target, statusType = _statusType, otherEntity = _other});
        Debug.Log($"State added successfully! {_target} has status {_statusType} towards {_other}");
    }

    public void RemoveState(StatusType _statusType, Entity _target, Entity _other = null)
    {
        if (ContainsState(_statusType, _target, _other) == false)
            return;
        
        State state = GetState(_statusType, _target, _other);
        states.Remove(state);
        Debug.Log($"State removed! {_target} has status {_statusType} towards {_other}");
    }
}
