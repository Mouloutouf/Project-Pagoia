using UnityEngine;

public enum GoalType { Mine = 0, Gather = 1, Fight = 2, Flee = 3 }

[CreateAssetMenu(fileName = "New Goal", menuName = "Scriptable Object/Goal")]
public class Goal : ScriptableObject
{
    public GoalType goalType;
    public int priority;

    public StateDefinition requiredState;
}
