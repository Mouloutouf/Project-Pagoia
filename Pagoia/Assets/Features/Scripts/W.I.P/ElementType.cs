using UnityEngine;

[CreateAssetMenu(fileName ="New Element", menuName = "Scriptable Object/Element")]
public class ElementType : ScriptableObject
{
    public GameObject prefab;

    public int size;
}
