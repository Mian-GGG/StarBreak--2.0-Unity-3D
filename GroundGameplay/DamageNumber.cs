using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatSpeed = 1.5f;
    public float fadeDuration = 1f;

    private float elapsed = 0f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    public void Setup(float damage, Vector3 worldPosition, Color color)
    {
        transform.position = worldPosition;
        text.text = Mathf.RoundToInt(damage).ToString();
        text.color = color;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        transform.forward = mainCam.transform.forward;
        float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        if (elapsed >= fadeDuration)
            Destroy(gameObject);
    }
}