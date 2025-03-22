using UnityEngine;

// TODO Should rewrite this code at some point to make a true non-overlapping random positions generator
public class TeamSpawner : MonoBehaviour
{
    public GameObject prefab;

    public int teamSize;
    public TeamDefinition team;

    public float radius;
    public Vector3[] areas;

    public float socialDistance;

    private Vector3[] spawnPositions;

    private void Start()
    {
        SpawnTeamInArea();
    }

    private Vector3 SelectRandomArea()
    {
        return areas[Random.Range(0, areas.Length)];
    }
    private void SpawnTeamInArea()
    {
        Vector3 randomArea = SelectRandomArea();

        spawnPositions = new Vector3[teamSize];
        for (var i = 0; i < teamSize; i++)
        {
            spawnPositions[i] = FindRandomSpawnPoint(randomArea);
            GameObject agentGameObject = SpawnAgent(spawnPositions[i]);

            Team agentTeam = agentGameObject.GetComponent<Team>();
            agentTeam.SetTeam(team);

            agentGameObject.name = $"Dwarf {team.name} {i}";
        }
    }
    private GameObject SpawnAgent(Vector3 _position)
    {
        return Instantiate(prefab, _position, Quaternion.identity, transform);
    }

    private Vector3 FindRandomSpawnPoint(Vector3 _areaCenter)
    {
        bool positionOverlap = false;

        Vector3 spawnPos = SelectRandomSpawnPoint(_areaCenter);
        for (int i = 0; i < teamSize; i++)
        {
            if (Vector3.Distance(spawnPositions[i], spawnPos) < socialDistance)
                positionOverlap = true;
        }
        if (positionOverlap == true)
            spawnPos = FindRandomSpawnPoint(_areaCenter);
        
        return spawnPos;
    }
    private Vector3 SelectRandomSpawnPoint(Vector3 _areaCenter)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius;
        Vector3 areaPos = randomPos + _areaCenter;
        
        return new Vector3(areaPos.x, 0, areaPos.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = team.colorMat.color;

        if (areas.Length == 0)
            return;

        foreach (Vector3 area in areas)
            Gizmos.DrawWireSphere(area, radius);
    }
}
