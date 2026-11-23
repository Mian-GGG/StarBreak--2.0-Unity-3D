using System.Collections.Generic;
using UnityEngine;

public class CockpitAnimationController : MonoBehaviour
{
    [Header("Joystick")]
    [SerializeField] private Transform _joystick;
    [SerializeField] private Vector3 _joystickRange = new Vector3(35f, 35f, 35f);

    [Header("Throttle")]
    [SerializeField] private List<Transform> _throttles = new List<Transform>();
    [SerializeField] private float _throttleRange = 35f;

    [Header("Input")]
    [SerializeField] private MovementControlsBase _movementControls;

    private IMovementControls Controls => _movementControls;

    void Update()
    {
        if (Controls == null) return;
        AnimateJoystick();
        AnimateThrottles();
    }

    void AnimateJoystick()
    {
        if (_joystick == null) return;
        _joystick.localRotation = Quaternion.Euler(
            Controls.PitchAmount * _joystickRange.x,
            Controls.YawAmount * _joystickRange.y,
            Controls.RollAmount * _joystickRange.z);
    }

    void AnimateThrottles()
    {
        float thrust = Controls.ThrustAmount * _throttleRange;
        foreach (var throttle in _throttles)
        {
            if (throttle == null) continue;
            Vector3 rot = throttle.localRotation.eulerAngles;
            rot.x = thrust;
            throttle.localRotation = Quaternion.Euler(rot);
        }
    }
}