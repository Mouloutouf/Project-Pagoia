using UnityEngine;

public class TeamSpawner : MonoBehaviour
{
    public Transform teamParent => this.transform;

    public GameObject prefab;

    public int teamSize;
    public TeamScriptable team;

    public float radius;
    public Vector3[] areas;

    public float socialDistance;

    private Vector3[] spawnPositions;

    private void Start()
    {
        SpawnTeamInArea();
    }

    #region Spawn Team
    private Vector3 SelectRandomArea()
    {
        return areas[Random.Range(0, areas.Length)];
    }
    public void SpawnTeamInArea()
    {
        var randomArea = SelectRandomArea();

        spawnPositions = new Vector3[teamSize];
        for (var i = 0; i < teamSize; i++)
        {
            spawnPositions[i] = FindRandomSpawnPoint(randomArea);
            var agentGameObject = SpawnAgent(spawnPositions[i]);

            var agentTeam = agentGameObject.GetComponent<Team>();
            agentTeam.SetTeam(team);

            agentGameObject.name = $"Dwarf {team.Name} {i}";
        }
    }
    private GameObject SpawnAgent(Vector3 _pos)
    {
        return Instantiate(prefab, _pos, Quaternion.identity, teamParent);
    }
    #endregion

    #region Find Spawn
    private Vector3 FindRandomSpawnPoint(Vector3 _area)
    {
        var invalid = false;

        SelectRandomSpawnPoint(_area, out var spawnPos);
        for (var u = 0; u < teamSize; u++)
        {
            if (spawnPositions[u] != null && 
                Vector3.Distance(spawnPositions[u], spawnPos) < socialDistance)
            {
                invalid = true;
            }
        }
        if (invalid)
        {
            spawnPos = FindRandomSpawnPoint(_area);
        }
        return spawnPos;
    }
    private void SelectRandomSpawnPoint(Vector3 _area, out Vector3 _spawnPos)
    {
        var randomPos = Random.insideUnitSphere * radius;

        var areaPos = randomPos + _area;
        _spawnPos = new Vector3(areaPos.x, 0, areaPos.z);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = team.colorMat.color;

        if (areas.Length == 0) return;

        foreach (var area in areas)
        {
            Gizmos.DrawWireSphere(area, radius);
        }
    }
    #endregion
}
