using UnityEngine;

public class VisionBehavior : MonoBehaviour
{
    public Agent agent;

    public float radius;
    private LayerMask layerMask;

    private void OnTriggerEnter(Collider _other)
    {
        var entity = _other.GetComponent<Entity>();
        World.instance.AddState(StateType.Knows, entity, agent.entity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
