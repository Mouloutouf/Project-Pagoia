using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    public Agent agent;

    public Inventory inventory;

    public float pickupRadius;
    public LayerMask autoPickupMask;

    private void Update()
    {
        var elements = Physics.OverlapSphere(this.transform.position, pickupRadius, autoPickupMask);

        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i].GetComponentInParent<Entity>();

            if (element.entityType == EntityType.OreCrystal)
            {
                inventory.crystals.Add(element);
                element.model.SetActive(false);
                World.instance.AddState(StateType.Equipped, element, agent.entity);
            }
            else if (element.entityType == EntityType.Pickaxe)
            {
                inventory.pickaxe = element;
                element.model.SetActive(false);
                World.instance.AddState(StateType.Equipped, element, agent.entity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, pickupRadius);
    }
}
