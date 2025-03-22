using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField]
    private TeamDefinition teamDefinition;
    [SerializeField]
    private MeshRenderer meshRenderer;

    public void SetTeam(TeamDefinition _teamDefinition)
    {
        teamDefinition = _teamDefinition;
        meshRenderer.material = _teamDefinition.colorMat;
    }
}
