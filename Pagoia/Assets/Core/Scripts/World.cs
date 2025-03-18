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
            Debug.LogError($"State {s.stateType} with entity {s.target.entityType}");
        }
    }

    private List<State> states = new List<State>();

    public Entity[] FindEntities(StateType _stateType, EntityType _entityType)
    {
        var entitySearchQuery =
            from state in states where (state.stateType == _stateType) && (state.target.entityType == _entityType) select state.target;

        return entitySearchQuery.ToArray();
    }
    public Entity[] FindEntities(EntityType _entityType, params (StateType stateType, bool shouldHaveState)[] _statesToConfirm)
    {
        // Start by Querying all the states that are related to that specific entity type
        var entitiesStatesQuery =
            from state in states where state.target.entityType == _entityType select state;

        // This is costly and quite heavy, but it's the only way to achieve what I want
        List<Entity> validEntities = new List<Entity>();
        List<Entity> invalidEntities = new List<Entity>();
        foreach (var entityState in entitiesStatesQuery)
        {
            foreach (var stateToConfirm in _statesToConfirm)
            {
                var condition = stateToConfirm.shouldHaveState
                    ? entityState.stateType == stateToConfirm.stateType
                    : entityState.stateType != stateToConfirm.stateType;
                
                if (condition)
                {
                    if (invalidEntities.Contains(entityState.target))
                        continue;
                    if (!validEntities.Contains(entityState.target))
                        validEntities.Add(entityState.target);
                }
                else
                {
                    if (!invalidEntities.Contains(entityState.target))
                        invalidEntities.Add(entityState.target);
                    if (validEntities.Contains(entityState.target))
                        validEntities.Remove(entityState.target);
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
                    from state in entitiesStatesQuery where state.stateType == stateToConfirm.stateType select state;
            }
            else
            {
                entitiesStatesQuery = 
                    from state in entitiesStatesQuery where state.stateType != stateToConfirm.stateType select state;
            }
            
            // stateQuery = 
            //     from state in stateQuery where stateToConfirm.shouldHaveState ? 
            //         state.stateType == stateToConfirm.stateType : 
            //         state.stateType != stateToConfirm.stateType 
            //     select state;
        }

        // End by querying the entity for each of the queried states
        var entitySearchQuery =
            from state in entitiesStatesQuery select state.target;

        return entitySearchQuery.ToArray();
    }

    public bool ContainsState(StateType _stateType, EntityType _entityType)
    {
        var searchedStateQuery =
            from state in states where (state.stateType == _stateType) && (state.target.entityType == _entityType) select state;
        //foreach (var s in states)
        //{
        //    Debug.LogError($"State {s.stateType} with entity {s.target.entityType}");
        //}
        //Debug.LogError($"State {_stateType} with entity {_entityType} is satisfied ? {searchedStateQuery.Any()}");

        return searchedStateQuery.Any();
    }
    public bool ContainsState(StateType _stateType, Entity _target)
    {
        var searchedStateQuery =
            from state in states where (state.stateType == _stateType) && (state.target == _target) select state;

        return searchedStateQuery.Any();
    }
    public bool ContainsState(StateType _stateType, Entity _target, Entity _actor)
    {
        var actorStateQuery =
            from state in states where (state.GetType() == typeof(ActorState)) select (ActorState)state;

        var searchedActorStateQuery =
            from actorState in actorStateQuery where (actorState.stateType == _stateType) && (actorState.target == _target) && (actorState.actor == _actor)
            select actorState;

        return searchedActorStateQuery.Any();
    }

    public State GetState(StateType _stateType, Entity _target)
    {
        var searchedStateQuery =
            from state in states where (state.stateType == _stateType) && (state.target == _target) select state;

        var searchedStates = searchedStateQuery.ToList();
        if (searchedStates.Any())
            return searchedStates.First();
        Debug.LogError($"There is no state identified as State {_stateType} with Target {_target.entityType}");
        return null;
    }
    public State GetState(StateType _stateType, Entity _target, Entity _actor)
    {
        var actorStateQuery =
            from state in states where (state.GetType() == typeof(ActorState)) select (ActorState)state;

        var searchedActorStateQuery =
            from actorState in actorStateQuery where (actorState.stateType == _stateType) && (actorState.target == _target) && (actorState.actor == _actor)
            select actorState;

        var searchedActorStates = searchedActorStateQuery.ToList();
        if (searchedActorStates.Any())
            return searchedActorStates.First();
        Debug.LogError($"There is no state identified as Actor {_actor.entityType} performing State {_stateType} to Target {_target.entityType}");
        return null;
    }

    public void AddState(StateType _stateType, Entity _target)
    {
        if (ContainsState(_stateType, _target)) return;
        states.Add(new State { stateType = _stateType, target = _target });
        Debug.Log($"State Added Successfully ! State {_stateType} with Target {_target}");
    }
    public void AddState(StateType _stateType, Entity _target, Entity _actor)
    {
        if (ContainsState(_stateType, _target, _actor)) return;
        states.Add(new ActorState { stateType = _stateType, target = _target, actor = _actor});
        Debug.Log($"State Added Successfully ! Actor {_actor} performing State {_stateType} to Target {_target}");
    }

    public void RemoveState(StateType _stateType, Entity _target)
    {
        if (!ContainsState(_stateType, _target)) return;
        
        var state = GetState(_stateType, _target);
        states.Remove(state);
        Debug.Log($"State Removed ! State {_stateType} with Target {_target}");
    }
    public void RemoveState(StateType _stateType, Entity _target, Entity _actor)
    {
        if (!ContainsState(_stateType, _target, _actor)) return;
        
        var state = GetState(_stateType, _target, _actor);
        states.Remove(state);
        Debug.Log($"State Removed ! Actor {_actor} performing State {_stateType} to Target {_target}");
    }
}
