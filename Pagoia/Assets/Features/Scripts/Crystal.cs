using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Transform crystal;
    public ElementType ElementType { get; set; }

    public void Pickup()
    {
        // DoTween
        // Grab / Suck Animation

        Debug.Log("Crystal Picked Up !");
        Destroy(crystal.gameObject);
    }
}
