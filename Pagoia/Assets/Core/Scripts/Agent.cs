using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : Entity
{
    public List<Goal> goals = new List<Goal>();
    
    private int priority; public int Priority { get => priority; set => priority = Mathf.Clamp(value, 0, orderedGoals.Count); }
    
    private List<Goal> orderedGoals = new List<Goal>();
    private Goal currentGoal;

    public List<ActionBehavior> availableActionBehaviors = new List<ActionBehavior>();

    private Action[] currentActionPlan;
    private int currentActionIndex;

    public bool Active { get; set; }

    private void Start()
    {
        orderedGoals = new List<Goal>(goals);
        orderedGoals.OrderBy(goal => goal.priority);

        Priority = 0;
        currentGoal = orderedGoals[Priority];
        
        StartPlan();
    }

    public void StartPlan()
    {
        currentActionPlan = Planner.CreatePlan(currentGoal, this, out bool success);

        if (success == false)
        {
            Debug.LogWarning($"Plan failed with goal {currentGoal} and Agent {this}");
            Priority++;
            
            if (Priority >= orderedGoals.Count)
            {
                Debug.LogWarning($"No more goals, Agent {this} dismissed");
                return; 
            }
            currentGoal = orderedGoals[Priority];
            
            StartPlan();
        }
        else
        {
            Active = true;
            foreach (var action in currentActionPlan)
            {
                Debug.LogWarning($"Action {action.actionData.name} with Target {action.target}");

                var actionBehaviorQuery =
                    from actionBehavior in availableActionBehaviors where (actionBehavior.actionData == action.actionData) select actionBehavior;
                action.actionBehavior = actionBehaviorQuery.Single();
            }

            currentActionIndex = 0;
            
            StartActionBehavior(currentActionIndex);
        }
    }

    public void StartActionBehavior(int _index)
    {
        var actionBehavior = currentActionPlan[_index].actionBehavior;
        actionBehavior.action = currentActionPlan[_index];
        actionBehavior.Active = true;
        
        actionBehavior.StartAction();
    }
    public void NextAction()
    {
        currentActionIndex++;
        if (currentActionIndex < currentActionPlan.Length)
        {
            StartActionBehavior(currentActionIndex);
        }
        else
        {
            Debug.Log("Hurray ! Goal is complete !");

            Priority = 0; // Do it again
            currentGoal = orderedGoals[Priority];
            
            StartPlan();
        }
    }
}
