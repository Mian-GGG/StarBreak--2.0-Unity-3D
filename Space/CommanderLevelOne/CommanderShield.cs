using System.Collections;
using UnityEngine;

public class CommanderShield : MonoBehaviour, ISpaceDamageable
{
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField][Range(0.25f, 1f)] private float _fadeOutTime = 0.5f;
    [SerializeField] private float _minIntensity = -10f;
    [SerializeField] private float _maxIntensity = 0f;

    private Renderer _renderer;
    private Color _baseColor;
    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _baseColor = _renderer.material.color;
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        StartCoroutine(FlashAndFade());
    }

    public void BreakShield()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    IEnumerator FlashAndFade()
    {
        float currentTime = 0f;
        while (currentTime < _fadeOutTime)
        {
            Color shieldColor = Color.Lerp(_flashColor, _baseColor, currentTime / _fadeOutTime);
            float intensity = Mathf.Lerp(_maxIntensity, _minIntensity, currentTime / _fadeOutTime);
            _renderer.material.color = shieldColor;
            _renderer.material.SetColor(EmissionColor, shieldColor * Mathf.Pow(2, intensity));
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
