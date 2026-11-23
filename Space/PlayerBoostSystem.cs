using UnityEngine;
using UnityEngine.UI;

public class PlayerBoostSystem : MonoBehaviour
{
    [SerializeField] DesktopMovementControls _movementControls;
    [SerializeField] Image _boostBarImage;
    [SerializeField] float _maxEnergy = 100f;
    [SerializeField] float _drainRate = 20f;
    [SerializeField] float _rechargeRate = 10f;
    [SerializeField] float _rechargeDelay = 1.5f;
    [SerializeField] GameObject _superSpeedParticleSystem;

    float _currentEnergy;
    float _rechargeTimer;
    bool _canBoost = true;

    void OnEnable()
    {
        _currentEnergy = _maxEnergy;
    }

    void Update()
    {
        if (_movementControls.BoostActive && _canBoost)
        {
            _currentEnergy -= _drainRate * Time.deltaTime;
            _rechargeTimer = _rechargeDelay;
            //_superSpeedParticleSystem.SetActive(true);
            //Debug.Log("Speeed");
            _superSpeedParticleSystem.GetComponent<ParticleSystem>().Play();

            if (_currentEnergy <= 0f)
            {
                _currentEnergy = 0f;
                _canBoost = false;
                _movementControls.CanBoost = false;
            }
        }
        else
        {
            _superSpeedParticleSystem.GetComponent<ParticleSystem>().Stop();
            //_superSpeedParticleSystem.SetActive(false);
            //Debug.Log("Speeed Off");
            _rechargeTimer -= Time.deltaTime;
            if (_rechargeTimer <= 0f)
            {
                _currentEnergy += _rechargeRate * Time.deltaTime;
                _currentEnergy = Mathf.Min(_currentEnergy, _maxEnergy);

                if (_currentEnergy >= _maxEnergy)
                {
                    _canBoost = true;
                    _movementControls.CanBoost = true;
                }
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (_boostBarImage == null) return;
        _boostBarImage.fillAmount = Mathf.Lerp(
            _boostBarImage.fillAmount,
            _currentEnergy / _maxEnergy,
            10f * Time.deltaTime);
    }
}