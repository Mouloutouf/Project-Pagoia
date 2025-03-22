using UnityEngine;

public class MoveAction : ActionBehavior
{
    public AgentMovement movement; public IMoveBehavior Movement => (IMoveBehavior)movement;

    public float distance;

    public Vector3 Destination => Target.transform.position;

    protected override bool Check()
    {
        var dist = Vector3.Distance(this.transform.position, Destination);
        return dist < distance;
    }

    public override void StartAction()
    {
        Movement.MoveTo(Destination);
    }
    public override void StopAction()
    {
        Movement.MoveTo(this.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, distance);
    }
}