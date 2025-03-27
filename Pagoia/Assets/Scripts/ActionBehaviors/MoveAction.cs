using UnityEngine;

public class MoveAction : ActionBehavior
{
    public AgentMovement movement;

    public float distance;

    public Vector3 Destination => Target.transform.position;

    protected override bool Check()
    {
        var dist = Vector3.Distance(transform.position, Destination);
        return dist < distance;
    }

    public override void StartAction()
    {
        movement.MoveTo(Destination);
    }
    public override void StopAction()
    {
        movement.MoveTo(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}