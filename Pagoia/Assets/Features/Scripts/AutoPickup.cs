using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    public Agent agent;

    public Inventory inventory;

    public float pickupRadius;
    public LayerMask autoPickupMask;

    private void Update()
    {
        var elements = Physics.OverlapSphere(transform.position, pickupRadius, autoPickupMask);

        foreach (var element in elements)
        {
            Pickable pickable = element.GetComponentInParent<Pickable>();
            
            pickable.model.SetActive(false);
            World.instance.AddState(StatusType.Inside, pickable, agent);

            // TODO Remove the state in physical space for element
            
            if (pickable is Resource resource)
                inventory.crystals.Add(resource);
            else if (pickable is Item item)
                inventory.pickaxe = item;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, pickupRadius);
    }
}
