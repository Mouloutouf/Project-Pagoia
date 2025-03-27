public class Action
{
    public ActionDefinition actionDefinition;
    public ActionContext actionContext;
    
    public ActionBehavior actionBehavior;

    public Entity target;

    public int Cost => CalculateActionCost();

    public StateDefinition[] requiredStates => actionContext.requiredStates;
    public StateDefinition[] satisfiedStates => actionContext.satisfiedStates;

    private int CalculateActionCost()
    {
        int cost = 0;

        foreach (StateDefinition stateDefinition in requiredStates)
        {
            // TODO Remake this whole method completely
            // if (stateDefinition.targetType == Target.New) {
            //     if (World.instance.ContainsState(stateDefinition.statusType, stateDefinition.entityType) == false) cost++;
            // }
            // else if (stateDefinition.targetType == Target.Same) {
            //     if (World.instance.ContainsState(stateDefinition.statusType, target) == false) cost++;
            // }
        }

        cost += actionDefinition.baseCost;
        return cost;
    }
}
