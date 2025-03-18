public class Action
{
    public ActionScriptable actionData;
    public ActionBehavior actionBehavior;

    public Entity target;

    public int Cost => CalculateActionCost();

    public StateTemplate[] requiredStates => actionData.requiredStates;
    public StateTemplate[] satisfiedStates => actionData.satisfiedStates;

    private int CalculateActionCost()
    {
        var cost = 0;

        foreach (var st in requiredStates)
        {
            if (st.targetType == Target.New) {
                if (World.instance.ContainsState(st.stateType, st.entityType) == false) cost++;
            }
            else if (st.targetType == Target.Same) {
                if (World.instance.ContainsState(st.stateType, target) == false) cost++;
            }
        }

        cost += actionData.baseCost;
        return cost;
    }
}
