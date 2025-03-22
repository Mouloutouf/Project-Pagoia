using UnityEngine;

public abstract class ActionBehavior : MonoBehaviour
{
    public Agent agent;

    public ActionDefinition actionData;
    public Action action { get; set; }

    public Entity Target => action.target;

    public bool Active { get; set; }

    public abstract void StartAction();
    public abstract void StopAction();
    protected abstract bool Check();

    private void Update()
    {
        if (Active == false)
            return;

        if (Check() == true)
        {
            StopAction();

            foreach (var stateDefinition in action.satisfiedStates)
            {
                if (stateDefinition.firstEntity.entityType.Has(Target.entityType))
                {
                    World.instance.AddState(stateDefinition.statusType, Target, agent);
                }
            }

            Active = false;
            agent.NextAction();
        }
    }
}
