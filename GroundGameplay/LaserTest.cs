using System.Collections;
using UnityEngine;

public class LaserTest : MonoBehaviour
{
    public Camera _camera;
    public Transform lasaerPoint;
    public float range = 100f, laserDuration = 3f;
    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            lr.SetPosition(0, lasaerPoint.position);
            RaycastHit hit;
            if(Physics.Raycast(lasaerPoint.position, lasaerPoint.transform.forward, out hit))
            {
                lr.SetPosition(1, hit.point);
                Debug.DrawRay(lasaerPoint.position, lasaerPoint.transform.forward, Color.green);
                Debug.Log("Laser");
            }
            StartCoroutine(Laser());
        }
        else
        {
            Debug.DrawRay(lasaerPoint.position, lasaerPoint.transform.forward, Color.red);
        }
    }

    IEnumerator Laser()
    {
        lr.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        lr.enabled = false;
    }
}
