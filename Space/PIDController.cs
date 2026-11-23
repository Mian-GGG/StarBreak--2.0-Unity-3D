using System;
using UnityEngine;

[Serializable]
public class PIDController
{
    public enum DerivativeMeasurement
    {
        Velocity,
        ErrorRateOfChange
    }

    [SerializeField] float _proportionalGain = 0.5f;
    [SerializeField] float _integralGain;
    [SerializeField] float _derivativeGain = 0.1f;
    [SerializeField] float _minOutput = -1f;
    [SerializeField] float _maxOutput = 1f;
    [SerializeField] float _integralSaturation;
    [SerializeField] DerivativeMeasurement _derivativeMeasurement;
    [SerializeField] bool _enablePid = true;

    public float _lastValue, _lastError, _integrationStored, _velocity;
    bool _derivativeInitialized;

    public void Reset()
    {
        _derivativeInitialized = false;
    }

    public float Update(float deltaTime, float currentValue, float targetValue)
    {
        if (!_enablePid) return targetValue;
        if (deltaTime <= 0) return targetValue;

        float error = targetValue - currentValue;
        float P = _proportionalGain * error;

        _integrationStored = Mathf.Clamp(
            _integrationStored + (error * deltaTime),
            -_integralSaturation,
            _integralSaturation
        );
        float I = _integralGain * _integrationStored;

        float errorRateOfChange = (error - _lastError) / deltaTime;
        _lastError = error;

        float valueRateOfChange = (currentValue - _lastValue) / deltaTime;
        _lastValue = currentValue;
        _velocity = valueRateOfChange;

        float deriveMeasure = 0;
        if (_derivativeInitialized)
        {
            deriveMeasure = _derivativeMeasurement == DerivativeMeasurement.Velocity
                ? -valueRateOfChange
                : errorRateOfChange;
        }
        else
        {
            _derivativeInitialized = true;
        }

        float D = _derivativeGain * deriveMeasure;
        return Mathf.Clamp(P + I + D, _minOutput, _maxOutput);
    }
}