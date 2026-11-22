using UnityEngine;

public class AlienLaser : MonoBehaviour
{
    Rigidbody rb;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveLaser(Vector3 _playerPos, float _laserForce)
    {
        
        Vector3 direction = (_playerPos - gameObject.transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);
        rb.linearVelocity = direction * _laserForce;
        gameObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        Debug.Log("Laser");
    }
}
