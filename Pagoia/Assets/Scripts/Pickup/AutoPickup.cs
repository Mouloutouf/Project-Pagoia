using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    public Agent agent;

    public Inventory inventory;

    public float pickupRadius;
    public LayerMask autoPickupMask;

    private void Update()
    {
        Collider[] elements = Physics.OverlapSphere(transform.position, pickupRadius, autoPickupMask);

        foreach (Collider element in elements)
        {
            Pickable pickable = element.GetComponentInParent<Pickable>();
            
            pickable.model.SetActive(false);
            World.instance.AddState(StatusType.Inside, pickable, agent);

            // TODO Remove state 'in physical space'
            
            inventory.AddElement(pickable);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
