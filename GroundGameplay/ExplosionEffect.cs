using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ExplosionEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
