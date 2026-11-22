using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FireEffect : MonoBehaviour
{
    void Start()
    {
        SnapToNavMesh();
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        
    }

    void SnapToNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            transform.position = hit.position;
    }
}
