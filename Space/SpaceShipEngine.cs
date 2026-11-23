using UnityEngine;

public class SpaceShipEngine : MonoBehaviour
{
    [SerializeField] GameObject _thruster;

    IMovementControls _shipMovementControls;
    Rigidbody _rigidbody;
    float _thrustForce;
    float _thrustAmount;

    bool ThrustersEnabled => !Mathf.Approximately(0f, _shipMovementControls.ThrustAmount);

    void Update()
    {
        ActivateThrusters();
    }

    void FixedUpdate()
    {
        if (!ThrustersEnabled) return;
        _rigidbody.AddForce(transform.forward * _thrustAmount * Time.fixedDeltaTime);
    }

    public void Init(IMovementControls movementControls, Rigidbody rb, float thrustForce)
    {
        _shipMovementControls = movementControls;
        _rigidbody = rb;
        _thrustForce = thrustForce;
    }

    void ActivateThrusters()
    {
        if (_shipMovementControls == null)
        {
            //Debug.Log("Movement controls is null on engine");
            return;
        }
        //Debug.Log($"Thrust amount: {_shipMovementControls.ThrustAmount}");
        _thruster.SetActive(ThrustersEnabled);
        if (!ThrustersEnabled) return;
        _thrustAmount = _thrustForce * _shipMovementControls.ThrustAmount;
    }

}