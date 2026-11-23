using UnityEngine;
using TMPro;

public class SpeedIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private string _unit = "m/s";
    [SerializeField] private bool _roundToInt = true;

    void LateUpdate()
    {
        if (_playerRigidbody == null || _speedText == null) return;

        float speed = _playerRigidbody.linearVelocity.magnitude;

        _speedText.text = _roundToInt ?
            $"{Mathf.RoundToInt(speed)} {_unit}" :
            $"{speed:F1} {_unit}";
    }
}