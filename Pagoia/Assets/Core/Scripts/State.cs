public enum StateType { Mining, Depositing, Equipped, Attacking, Exists, At, Knows, Crafting }

// TODO Create only one state class with a first entity, a situation or state type, and a second entity
// TODO Change the State Type with the new situations we described in the miro

public class State
{
    public StateType stateType;

    public Entity target;
}

public class ActorState : State
{
    public Entity actor;
}
