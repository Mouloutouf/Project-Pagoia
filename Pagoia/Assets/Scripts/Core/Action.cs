public class Action
{
    public ActionDefinition actionData;
    public ActionBehavior actionBehavior;

    public Entity target;

    public int Cost => CalculateActionCost();

    public StateDefinition[] requiredStates => actionData.requiredStates;
    public StateDefinition[] satisfiedStates => actionData.satisfiedStates;

    private int CalculateActionCost()
    {
        var cost = 0;

        foreach (var stateDefinition in requiredStates)
        {
            // TODO Remake this whole method completely
            // if (stateDefinition.targetType == Target.New) {
            //     if (World.instance.ContainsState(stateDefinition.statusType, stateDefinition.entityType) == false) cost++;
            // }
            // else if (stateDefinition.targetType == Target.Same) {
            //     if (World.instance.ContainsState(stateDefinition.statusType, target) == false) cost++;
            // }
        }

        cost += actionData.baseCost;
        return cost;
    }
}
