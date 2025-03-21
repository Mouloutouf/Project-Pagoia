using UnityEngine;

public class Pickable : Entity
{
    public void Pickup()
    {
        // DoTween
        // Grab / Suck Animation

        Debug.Log("Pickable Picked Up !");
        Destroy(gameObject);
    }
}
