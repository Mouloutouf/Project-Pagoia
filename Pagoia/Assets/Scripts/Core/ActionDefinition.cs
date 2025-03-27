using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionContext
{
    public StateDefinition[] requiredStates;
    
    public StateDefinition[] satisfiedStates;
    public StateDefinition[] clearedStates;
}

[CreateAssetMenu(fileName = "New Action Definition", menuName = "Scriptable Object/Action")]
public class ActionDefinition : ScriptableObject
{
    public string actorName;

    public List<ActionContext> contexts;
    
    public int baseCost;
}
