using UnityEngine;
using UnityEngine.UI;

public class SpaceHealthBar : MonoBehaviour
{
    [SerializeField] Image _healthBarImage;
    [SerializeField] float _updateRate = 1f;
    [SerializeField] SpaceHealthHandler _healthHandler;

    Transform _transform;
    float _targetFillAmount;

    void Awake()
    {
        _transform = transform;
    }

    void OnEnable()
    {
        if (_healthHandler == null) return;
        _healthHandler.HealthChanged.AddListener(UpdateHealthBar);
        _healthHandler.ObjectDestroyed.AddListener(DisableHealthBar);
        _targetFillAmount = 1f;
    }

    void OnDisable()
    {
        if (_healthHandler == null) return;
        _healthHandler.HealthChanged.RemoveListener(UpdateHealthBar);
        _healthHandler.ObjectDestroyed.RemoveListener(DisableHealthBar);
    }

    void LateUpdate()
    {
        _transform.LookAt(Camera.main.transform);

        if (_healthBarImage == null) return;
        if (Mathf.Approximately(_healthBarImage.fillAmount, _targetFillAmount)) return;
        _healthBarImage.fillAmount = Mathf.MoveTowards(
            _healthBarImage.fillAmount,
            _targetFillAmount,
            _updateRate * Time.deltaTime);
    }

    void UpdateHealthBar()
    {
        if (_healthHandler == null) return;
        if (_healthHandler.Health <= 0)
        {
            _targetFillAmount = 0f;
            return;
        }
        _targetFillAmount = (float)_healthHandler.Health / _healthHandler.MaxHealth;
    }

    void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }
}