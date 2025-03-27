using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Planner
{
    public static Action[] CreatePlan(GoalDefinition goalDefinition, Agent _agent)
    {
        Debug.LogWarning($"Starting Plan for Goal {goalDefinition.name} with Agent {_agent}");

        List<ActionDefinition> availableActions = (from actionBehavior in _agent.availableActionBehaviors select actionBehavior.actionDefinition).ToList();

        Action[] plan = FindPlan(out bool status, availableActions, goalDefinition.requiredState, _agent);
        return status == true ? plan : null;
    }

    private static Entity FindValidTarget(EntityDefinition _entityDefinition, Agent _agent)
    {
        Entity[] potentialTargets = World.instance.FindEntities(_entityDefinition.entityType, (StatusType.Exists, true), (StatusType.InPhysicalSpace, true));
        
        if (potentialTargets.Length > 0)
        {
            // TODO Make sure we have different ways of determining the best entity based on the actual action we are trying to perform
            
            // Pick the closest target out of all entities
            float[] distances = new float[potentialTargets.Length];
            for (int i = 0; i < potentialTargets.Length; i++)
            {
                distances[i] = Vector3.Distance(_agent.transform.position, potentialTargets[i].transform.position);
                //Debug.LogWarning($"Distance to target {potentialTargets[i]} is {distances[i]} with {_agent.transform.position}");
            }
            float minDist = distances.Min();
            int closest = Array.IndexOf(distances, minDist);
            
            //potentialTargets.OrderBy(target => Vector3.Distance(_agent.transform.position, target.transform.position));
            //target = potentialTargets.First();
            //Debug.LogWarning($"Closest to target {potentialTargets[closest]} with {minDist}");
            
            Entity validTarget = potentialTargets[closest];
            Debug.Log($"Target found ! Target is {validTarget}");

            return validTarget;
        }
        
        Debug.Log($"Target not found. No entities of type {_entityDefinition.entityType} could be found");
        return null;
    }

    // TODO Rework the Search to use a breadth first search using a queue data structure
    // TODO Make sure that if we find a successful path (a path that ends that is), and that path has a better cost than the other paths still not fully explored, then we stop the search
    // TODO Implement a max depth for the agent that can be modified for performances, which will stop the search if and only if it has at least one successful path that is not the best in cost
    // We might as well do a cost to depth ratio where we have an exit cost which decreases as the depth increases, in order to always eventually exit with a plan even if the paths are all long
    // TODO Implement an absolute depth at which the search system will stop, and if no complete path is found by then, will return a failed plan
    // Tries to find the best plan possible, with the given actions, based on the required state, and the given target
    private static Action[] FindPlan(out bool _status, List<ActionDefinition> _availableActions, StateDefinition _requiredState, Agent _agent, Entity _target = null)
    {
        if (_target == null)
        {
            _target = FindValidTarget(_requiredState.firstEntity, _agent);
            
            if (_target == null)
            {
                Debug.LogWarning($"The Goal required cannot be performed, no valid entities were found");
                _status = false;
                return null;
            }
        }
        
        Debug.Log($"Starting Search for State {_requiredState.statusType} with Target {_target}");
        
        // Check if the state required is already satisfied, if so then the required state does not need any action plan
        // The state is already achieved, the plan is returned empty
        // In case the state required is considered satisfied for any entity that satisfies it
        // TODO Remake this to use the ids correctly
        if (_requiredState.firstEntity.entityId == _target.name)
        {
            if (World.instance.ContainsState(_requiredState.statusType, _target.entityType))
            {
                Debug.Log($"State {_requiredState.statusType} with Target Type {_target.entityType} is already satisfied");
                _status = true;
                return null;
            }
        }
        // In case the state required is considered satisfied if the target entity satisfies it
        else
        {
            if (World.instance.ContainsState(_requiredState.statusType, _target))
            {
                Debug.Log($"State {_requiredState.statusType} with Target {_target} is already satisfied");
                _status = true;
                return null;
            }
        }

        List<Action> potentialPlan = new List<Action>();

        List<ActionContext> potentialActions = GetValidActionsForRequiredState(_availableActions, _requiredState, _target);

        // Pick the best action out of all possibilities
        Action bestAction = PickBestAction(potentialActions.ToArray());
        
        // Check if the picked action has any required states
        foreach (StateDefinition state in bestAction.requiredStates)
        {
            Action[] additionalPlan;

            // Recursion process to find a plan that satisfies the new required state
            // TODO Remake this check using the entity ids
            if (true /*requiredActionStates[i].targetType == Target.New*/)
                additionalPlan = FindPlan(out _status, _availableActions, state, _agent);
            else
                additionalPlan = FindPlan(out _status, _availableActions, state, _agent, _target);

            // If plan failed return immediately
            if (_status == false)
                return null;
            
            // Otherwise add the additional plan if any
            if (additionalPlan != null)
                potentialPlan.AddRange(additionalPlan);
        }

        // Add the best action for this required state last
        potentialPlan.Add(bestAction);

        // Return the Plan
        _status = true;
        return potentialPlan.ToArray();
    }

    private static Action PickBestAction(Action[] _actions)
    {
        var orderedActions = _actions.OrderBy(_action => _action.Cost);

        Action action = orderedActions.First();
        Debug.Log($"Picking Action {action.actionDefinition.name} with Target {action.target}");
        
        return action;
    }

    private static List<ActionContext> GetValidActionsForRequiredState(List<ActionDefinition> _actions, StateDefinition _requiredState, in Entity _target)
    {
        List<ActionContext> validActions = new List<ActionContext>();

        foreach (ActionDefinition actionDefinition in _actions)
        {
            foreach (ActionContext actionContext in actionDefinition.contexts)
            {
                foreach (StateDefinition satisfiedState in actionContext.satisfiedStates)
                {
                    if (satisfiedState == _requiredState)
                        validActions.Add(actionContext);
                }
            }
        }
        return validActions;
    }
}
