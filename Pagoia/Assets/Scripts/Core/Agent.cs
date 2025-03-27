using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : Entity
{
    public List<GoalDefinition> goals = new List<GoalDefinition>();
    
    private int priority; public int Priority { get => priority; set => priority = Mathf.Clamp(value, 0, orderedGoals.Count); }
    
    private List<GoalDefinition> orderedGoals = new List<GoalDefinition>();
    private GoalDefinition currentGoalDefinition;

    public List<ActionBehavior> availableActionBehaviors = new List<ActionBehavior>();

    private Action[] currentActionPlan;
    private int currentActionIndex;

    public bool Active { get; set; }

    private void Start()
    {
        orderedGoals = new List<GoalDefinition>(goals);
        orderedGoals.OrderBy(goal => goal.priority);

        Priority = 0;
        currentGoalDefinition = orderedGoals[Priority];
        
        StartPlan();
    }

    public void StartPlan()
    {
        currentActionPlan = Planner.CreatePlan(currentGoalDefinition, this);

        if (currentActionPlan == null)
        {
            Debug.LogWarning($"Plan failed with goal {currentGoalDefinition} and Agent {this}");
            Priority++;
            
            if (Priority >= orderedGoals.Count)
            {
                Debug.LogWarning($"No more goals, Agent {this} dismissed");
                return;
            }
            currentGoalDefinition = orderedGoals[Priority];
            
            StartPlan();
        }
        else
        {
            Active = true;
            foreach (Action action in currentActionPlan)
            {
                Debug.LogWarning($"Action {action.actionDefinition.name} with Target {action.target}");

                action.actionBehavior = (from actionBehavior in availableActionBehaviors
                    where actionBehavior.actionDefinition == action.actionDefinition
                    select actionBehavior).Single();
            }

            currentActionIndex = 0;
            
            StartActionBehavior(currentActionIndex);
        }
    }

    public void StartActionBehavior(int _index)
    {
        ActionBehavior actionBehavior = currentActionPlan[_index].actionBehavior;
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
            currentGoalDefinition = orderedGoals[Priority];
            
            StartPlan();
        }
    }
}
