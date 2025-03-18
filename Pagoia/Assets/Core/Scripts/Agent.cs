using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Entity entity;

    public List<Goal> goals = new List<Goal>();
    private List<Goal> currentGoals = new List<Goal>();
    private int priority; public int Priority { get => priority; set => priority = Mathf.Clamp(value, 0, currentGoals.Count); }
    private Goal current;

    public List<ActionBehavior> availableActionBehaviors = new List<ActionBehavior>();

    private Action[] currentActionPlan;
    private int currentActionIndex;

    public bool active { get; set; }

    private void Start()
    {
        foreach (var goal in goals) {
            currentGoals.Add(goal);
        }
        currentGoals.OrderBy(_goal => _goal.priority);

        Priority = 0;
        current = currentGoals[Priority];
        StartPlan();
    }

    public void StartPlan()
    {
        currentActionPlan = Planner.CreatePlan(current, this, out bool success);

        if (!success)
        {
            Debug.LogWarning($"Plan failed with goal {current} and Agent {this}");
            Priority++;
            if (Priority >= currentGoals.Count)
            {
                Debug.LogWarning($"No more goals, Agent {this} dismissed");

                return; 
            }
            current = currentGoals[Priority];
            StartPlan();
        }
        else
        {
            active = true;
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
            current = currentGoals[Priority];
            StartPlan();
        }
    }
}
