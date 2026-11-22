using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance;
    public GameObject damageNumberPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(float damage, Vector3 position)
    {
        Vector3 offset = new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(0.5f, 1f),
            Random.Range(-0.3f, 0.3f)
        );

        Color color = damage < 15f ? Color.blue : Color.red;

        GameObject obj = Instantiate(damageNumberPrefab, position + offset, Quaternion.identity);
        obj.GetComponent<DamageNumber>().Setup(damage, position + offset, color);
    }
}