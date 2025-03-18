public enum StateType { Mining, Depositing, Equipped, Attacking, Exists, At, Knows, Crafting }

public class State
{
    public StateType stateType;

    public Entity target;
}

public class ActorState : State
{
    public Entity actor;
}
