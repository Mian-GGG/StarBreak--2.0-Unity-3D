using UnityEngine;
using UnityEngine.UI;

public class PlayerBlasterUI : MonoBehaviour
{
    [SerializeField] Image _capacitorBar;
    [SerializeField] Blaster[] _blasters;

    void LateUpdate()
    {
        if (_blasters.Length == 0) return;

        float totalCharge = 0f;
        foreach (var blaster in _blasters)
            totalCharge += blaster.CapacitorChargePercentage;

        float averageCharge = totalCharge / _blasters.Length;

        _capacitorBar.fillAmount = Mathf.Lerp(
            _capacitorBar.fillAmount,
            averageCharge,
            10f * Time.deltaTime);
    }
}