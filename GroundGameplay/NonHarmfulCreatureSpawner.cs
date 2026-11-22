using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NonHarmfulCreatureSpawner : MonoBehaviour
{
    public GameObject _creature;
    public int _creatureCount = 25;
    public float spawnRadius = 150f;


    void Start()
    {
        Spawner();
    }

    public void Spawner()
    {
        for(int i = 0; i < _creatureCount; i++)
        {
            Vector3 spawnPos = GetRandomPointOnNavMesh(transform.position, spawnRadius);
            Instantiate(_creature, spawnPos, Quaternion.identity, gameObject.transform);
        }
    }

    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++) 
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return center;
    }

   
}
