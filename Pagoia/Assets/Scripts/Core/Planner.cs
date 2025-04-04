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

    public static void Plan(List<ActionDefinition> _availableActions, StateDefinition _requiredState)
    {
        Queue<(int, ActionContext)> actionQueue = new Queue<(int, ActionContext)>();
        List<List<StateDefinition>> validatedStatesOfPaths = new List<List<StateDefinition>>();
        List<List<ActionContext>> actionPaths = new List<List<ActionContext>>();
        
        List<ActionContext> potentialActions = GetValidActionsForRequiredState(_availableActions, _requiredState);
        
        for (int i = 0; i < potentialActions.Count; i++)
        {
            actionPaths.Add(new List<ActionContext>());
            validatedStatesOfPaths.Add(new List<StateDefinition>());
            
            int newPath = actionPaths.Count;
            actionQueue.Enqueue((newPath, potentialActions[i]));
        }
        
        // Go through the actions in a breadth first search order
        while (actionQueue.Count > 0)
        {
            // For a given action within a specific path
            var(currentPath, currentAction) = actionQueue.Peek();

            // Go through all required states for the current action in descending order of priority, since higher priority actions will be performed first, we check them last
            List<StateDefinition> requiredStates = currentAction.requiredStates.OrderByDescending(_state => _state.priority).ToList();
            
            foreach (StateDefinition state in requiredStates)
            {
                // Check if the state is already satisfied, if it is, move on to the next state
                bool isSatisfied = CheckIfStateIsSatisfied(currentPath, state);

                if (isSatisfied == true)
                    continue;
                
                // Search for all potential actions that can satisfy the state we want
                List<ActionContext> actionsForRequiredState = GetValidActionsForRequiredState(_availableActions, state);
                
                // Go through all these actions
                for (int i = 0; i < actionsForRequiredState.Count; i++)
                {
                    ActionContext action = actionsForRequiredState[i];
                    int path;
                    
                    // If we have just one action then the path does not change
                    if (i == 0)
                    {
                        path = currentPath;
                    }
                    // If we have more then subsequent actions will generate a new path each
                    else
                    {
                        // Copy everything from the current path to the new path
                        actionPaths.Add(new List<ActionContext>(actionPaths[currentPath]));
                        validatedStatesOfPaths.Add(new List<StateDefinition>(validatedStatesOfPaths[currentPath]));

                        // New path index is equal to the new size
                        path = actionPaths.Count;
                    }
                    
                    // Add the action to the queue
                    actionQueue.Enqueue((path, action));
                    
                    // Add the action to the list of actions for its path
                    actionPaths[path].Add(action);
                    
                    // Add all the satisfied states this action accomplishes to the validated states of its path
                    foreach (StateDefinition satisfiedState in action.satisfiedStates)
                    {
                        validatedStatesOfPaths[path].Add(satisfiedState);
                    }
                }
            }
            
            // Remove the current action from the queue
            actionQueue.Dequeue();
        }
    }

    private static bool CheckIfStateIsSatisfied(int path, StateDefinition state)
    {
        return state.priority > 1 && path > 2;
    }
    
    private static Action[] FindPlan(out bool _status, List<ActionDefinition> _availableActions, StateDefinition _requiredState, Agent _agent, Entity _target = null)
    {
        // TODO Remove this system entirely
        #region Find Target(s) for Action

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

        #endregion

        // TODO Remake this system entirely
        #region Check For Satisfied State

        // In case the state required is considered satisfied for any entity that satisfies it
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

        #endregion

        #region Breadth First Search

        Debug.Log($"Starting Search for State {_requiredState.statusType} with Target {_target}");
        
        Queue<ActionContext> queue = new Queue<ActionContext>();
        
        List<Action> potentialPlan = new List<Action>();

        List<ActionContext> potentialActions = GetValidActionsForRequiredState(_availableActions, _requiredState);

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

        #endregion
    }

    private static Action PickBestAction(Action[] _actions)
    {
        var orderedActions = _actions.OrderBy(_action => _action.Cost);

        Action action = orderedActions.First();
        Debug.Log($"Picking Action {action.actionDefinition.name} with Target {action.target}");
        
        return action;
    }

    private static List<ActionContext> GetValidActionsForRequiredState(List<ActionDefinition> _actions, StateDefinition _requiredState)
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
