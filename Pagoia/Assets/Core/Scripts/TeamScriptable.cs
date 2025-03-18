using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Scriptables/Team")]
public class TeamScriptable : ScriptableObject
{
    public Material colorMat;
    public string Name => name;
}
