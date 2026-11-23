using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Image _healthBarImage;
    [SerializeField] SpaceHealthHandler _healthHandler;
    float _targetFill;

    void OnEnable()
    {
        _healthHandler.HealthChanged.AddListener(UpdateBar);
        _targetFill = 1f;
    }

    void OnDisable()
    {
        _healthHandler.HealthChanged.RemoveListener(UpdateBar);
    }

    void UpdateBar()
    {
        _targetFill = (float)_healthHandler.Health / _healthHandler.MaxHealth;
    }

    void LateUpdate()
    {
        _healthBarImage.fillAmount = Mathf.MoveTowards(
            _healthBarImage.fillAmount, _targetFill, Time.deltaTime);
    }
}