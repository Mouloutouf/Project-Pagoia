using UnityEngine;

[CreateAssetMenu(fileName = "New Goal Definition", menuName = "Scriptable Object/Goal")]
public class GoalDefinition : ScriptableObject
{
    public int priority;

    public StateDefinition requiredState;
}
