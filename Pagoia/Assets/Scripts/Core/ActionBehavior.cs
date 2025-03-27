using UnityEngine;
using UnityEngine.Serialization;

public abstract class ActionBehavior : MonoBehaviour
{
    public Agent agent;

    [FormerlySerializedAs("actionData")] public ActionDefinition actionDefinition;
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

            foreach (StateDefinition stateDefinition in action.satisfiedStates)
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
