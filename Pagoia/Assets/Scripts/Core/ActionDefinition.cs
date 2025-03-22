using UnityEngine;

[CreateAssetMenu(fileName = "New Action Definition", menuName = "Scriptable Object/Action")]
public class ActionDefinition : ScriptableObject
{
    public string actorName;

    public StateDefinition[] requiredStates;
    
    public StateDefinition[] satisfiedStates;
    public StateDefinition[] clearedStates;
    
    public int baseCost;
}
