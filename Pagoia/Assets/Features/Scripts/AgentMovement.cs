using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour, IMoveBehavior
{
    public NavMeshAgent navMeshAgent;

    public void MoveTo(Vector3 _dest)
    {
        navMeshAgent.SetDestination(_dest);
    }
}
