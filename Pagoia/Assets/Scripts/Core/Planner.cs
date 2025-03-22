using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Planner
{
    public static Action[] CreatePlan(GoalDefinition goalDefinition, Agent _agent, out bool _status)
    {
        Action[] plan;
        Debug.LogWarning($"Starting Plan for Goal {goalDefinition.name} with Agent {_agent}");

        // Finding all the agent's available actions
        var availableActionsQuery =
            from actionBehavior in _agent.availableActionBehaviors select actionBehavior.actionData;
        var availableActions = availableActionsQuery.ToList();

        // Start the Planner
        plan = FindPlan(availableActions, goalDefinition.requiredState, _agent, out _status);

        // If the plan is empty, it means it has failed at some point in the process
        // Set the status to tell if it has succeeded or not
        // Return the plan
        _status = (plan != null);
        return plan;
    }

    private static Entity FindValidTarget(EntityType _entityType, Agent _agent)
    {
        // Find all entities of this type
        // In this specific case -> Find all entities of a certain type that exist and that are in physical space
        var potentialTargets = World.instance.FindEntities(_entityType, (StatusType.Exists, true), (StatusType.InPhysicalSpace, true));
        Entity target;
        if (potentialTargets.Length > 0)
        {
            // Pick the closest target out of all entities
            var distances = new float[potentialTargets.Length];
            for (var i = 0; i < potentialTargets.Length; i++)
            {
                distances[i] = Vector3.Distance(_agent.transform.position, potentialTargets[i].transform.position);
                //Debug.LogWarning($"Distance to target {potentialTargets[i]} is {distances[i]} with {_agent.transform.position}");
            }
            var minDist = distances.Min();
            var closest = Array.IndexOf(distances, minDist);
            //Debug.LogWarning($"Closest to target {potentialTargets[closest]} with {minDist}");
            target = potentialTargets[closest];

            //potentialTargets.OrderBy(target => Vector3.Distance(_agent.transform.position, target.transform.position));
            //target = potentialTargets.First();

            Debug.Log($"Target found ! Target is {target}");
        }
        else
        {
            target = null;
            Debug.Log($"Target not found. No entities of type {_entityType} could be found");
        }
        return target;
    }

    private static Action[] FindPlan(List<ActionDefinition> _availableActions, StateDefinition _requiredState, Agent _agent, out bool _status)
    {
        // Check if there is a valid target for the required state
        var target = FindValidTarget(_requiredState.firstEntity.entityType, _agent);

        // If there is not, then the goal cannot be achieved, the plan is returned empty
        if (target == null) {
            Debug.LogWarning($"The Goal required cannot be performed, no valid entities were found");
            _status = false;
            return null;
        }

        // Start the Search, with the right target
        Debug.Log($"Starting Search for State {_requiredState.statusType} with Target {target}");
        SearchPlan(_availableActions, _requiredState, out var plan, target, _agent, out _status);
        return plan;
    }

    private static Action[] FindPlan(List<ActionDefinition> _availableActions, StateDefinition _requiredState, Entity _target, Agent _agent, out bool _status)
    {
        // Start the Search, with the right target
        Debug.Log($"Starting Search for State {_requiredState.statusType} with Target {_target}");
        SearchPlan(_availableActions, _requiredState, out var plan, _target, _agent, out _status);
        return plan;
    }

    // Tries to find the best plan possible, with the given actions, based on the required state, and the given target
    private static void SearchPlan(List<ActionDefinition> _availableActions, StateDefinition _requiredState, 
        out Action[] _plan, in Entity _target, in Agent _agent, out bool _status)
    {
        // Set the status to true. Will become false if the plan fails
        _status = true;

        // Check if the state required is already satisfied, if so then the required state does not need any action plan
        // The state is already achieved, the plan is returned empty
        // In case the state required is considered satisfied for any entity that satisfies it
        // TODO Remake this to use the ids correctly
        if (_requiredState.firstEntity.entityId == _target.name) {
            if (World.instance.ContainsState(_requiredState.statusType, _target.entityType)) {
                Debug.Log($"State {_requiredState.statusType} with Target Type {_target.entityType} is already satisfied");
                _plan = null; return;
            }
        }
        // In case the state required is considered satisfied if the target entity satisfies it
        else {
            if (World.instance.ContainsState(_requiredState.statusType, _target)) {
                Debug.Log($"State {_requiredState.statusType} with Target {_target} is already satisfied");
                _plan = null; return;
            }
        }

        var potentialPlan = new List<Action>();

        // Find actions that satisfy the required state
        var potentialActions = SearchForActions(_availableActions, _requiredState, _target);

        // Pick the best action out of all possibilities
        var bestAction = PickBestAction(potentialActions);
        
        // Check if the picked action has any required states
        var requiredActionStates = bestAction.actionData.requiredStates;
        if (requiredActionStates.Length > 0) {
            for (var i = 0; i < requiredActionStates.Length; i++)
            {
                Action[] additionalPlan;

                // Recursion process to find a plan that satisfies the new required state
                // TODO Remake this check using the entity ids
                if (true /*requiredActionStates[i].targetType == Target.New*/)
                    additionalPlan = FindPlan(_availableActions, requiredActionStates[i], _agent, out _status);
                else
                    additionalPlan = FindPlan(_availableActions, requiredActionStates[i], _target, _agent, out _status);

                // If no plan is found
                if (additionalPlan == null)
                {
                    // Then we check if the plan failed, if so, then the goal cannot be achieved, the plan is returned empty
                    if (_status == false) {
                        _plan = null; _status = false; return;
                    }
                    // Otherwise, just continue with the next required state
                    continue;
                }
                // Otherwise, the plan is added to the current plan
                potentialPlan.AddRange(additionalPlan);
            }
        }

        // Add the best action for this required state last
        potentialPlan.Add(bestAction);

        // Return the Plan
        _plan = potentialPlan.ToArray();
    }

    private static Action PickBestAction(Action[] _actions)
    {
        // This will calculate the costs of each Action, then order them. The Cost is a property for a Calculate Cost method
        var orderedActions = _actions.OrderBy(_action => _action.Cost);

        var action = orderedActions.First();
        Debug.Log($"Picking Action {action.actionData.name} with Target {action.target}");
        return action;
    }

    private static Action[] SearchForActions(List<ActionDefinition> _actionsToSearch, StateDefinition _requiredState, in Entity _target)
    {
        var actionsFound = new List<Action>();

        foreach (var actionTemplate in _actionsToSearch)
        {
            for (var i = 0; i < actionTemplate.satisfiedStates.Length; i++) {
                if ((actionTemplate.satisfiedStates[i].statusType == _requiredState.statusType) && 
                    (actionTemplate.satisfiedStates[i].firstEntity.entityType.Has(_requiredState.firstEntity.entityType)))
                {
                    Debug.Log($"New potential Action {actionTemplate.name} with Target {_target}");
                    var action = new Action { actionData = actionTemplate, target = _target };
                    actionsFound.Add(action);
                }
            }
        }
        return actionsFound.ToArray();
    }
}
