using UnityEngine;

public abstract class ActionBehavior : MonoBehaviour
{
    public Agent agent;

    public ActionScriptable actionData;
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

            foreach (var st in action.satisfiedStates)
            {
                if (st.entityType.Has(Target.entityType))
                {
                    World.instance.AddState(st.stateType, Target, agent.entity);
                }
            }

            Active = false;
            agent.NextAction();
        }
    }
}
