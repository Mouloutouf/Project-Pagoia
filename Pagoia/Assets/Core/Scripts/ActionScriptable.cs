using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Scriptable Object/Action")]
public class ActionScriptable : ScriptableObject
{
    [EnumFlags(typeof(EntityType))]
    public EntityType target;

    public int baseCost;

    public StateTemplate[] requiredStates;
    public StateTemplate[] satisfiedStates;
}
