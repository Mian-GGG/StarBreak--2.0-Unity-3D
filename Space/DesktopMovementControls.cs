using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopMovementControls : MovementControlsBase
{
    [SerializeField] float _deadZoneRadius = 0.1f;
    [SerializeField] float _boostMultiplier = 2f;
    float _rollAmount;

    public bool CanBoost { get; set; } = true;

    bool IsBoosting => Keyboard.current.leftShiftKey.isPressed && CanBoost;

    Vector2 ScreenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    public bool BoostActive => IsBoosting && ThrustAmount > 0f;

    public override float YawAmount
    {
        get
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float yaw = (mousePosition.x - ScreenCenter.x) / ScreenCenter.x;
            return Mathf.Abs(yaw) > _deadZoneRadius ? yaw : 0f;
        }
    }

    public override float PitchAmount
    {
        get
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float pitch = (mousePosition.y - ScreenCenter.y) / ScreenCenter.y;
            return Mathf.Abs(pitch) > _deadZoneRadius ? pitch * -1f : 0f;
        }
    }

    public override float RollAmount
    {
        get
        {
            float roll = Keyboard.current.qKey.isPressed ? 1f :
                         Keyboard.current.eKey.isPressed ? -1f : 0f;
            _rollAmount = Mathf.Lerp(_rollAmount, roll, Time.deltaTime * 3f);
            return _rollAmount;
        }
    }

    public void SetDeadZone(float value)
    {
        _deadZoneRadius = value;
    }

    public override float ThrustAmount
    {
        get
        {
            float thrust = Keyboard.current.wKey.isPressed ? 1f :
                           Keyboard.current.sKey.isPressed ? -1f : 0f;
            return IsBoosting && thrust > 0f ? thrust * _boostMultiplier : thrust;
        }
    }
}