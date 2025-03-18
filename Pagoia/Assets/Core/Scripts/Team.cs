using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] // TODO Create a ReadOnly custom attribute to make it visible but not editable in the inspector
    private TeamScriptable teamData;
    [SerializeField]
    private MeshRenderer meshRenderer;

    public void SetTeam(TeamScriptable _teamData)
    {
        teamData = _teamData;
        meshRenderer.material = _teamData.colorMat;
    }
}
