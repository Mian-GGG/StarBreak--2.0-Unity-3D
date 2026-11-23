using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Healing")]
    [SerializeField] private int _healAmount = 200;
    [SerializeField] protected bool _isGround, _isAmmo;

    [Header("Movement")]
    [SerializeField] private float _rotateSpeed = 90f;
    [SerializeField] private float _bobHeight = 0.5f;
    [SerializeField] private float _bobSpeed = 1f;

    [Header("Lifetime")]
    [SerializeField] private float _lifetime = 15f;

    private Vector3 _startPosition;
    private float _elapsed;

    void Start()
    {
        _startPosition = transform.position;
        Destroy(gameObject, _lifetime);
    }

    void Update()
    {
        _elapsed += Time.deltaTime;

        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime, Space.World);

        Vector3 pos = _startPosition;
        pos.y += Mathf.Sin(_elapsed * _bobSpeed) * _bobHeight;
        transform.position = pos;
    }

    private bool _hasHealed = false;

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + other.gameObject.tag);
        //Debug.Log("Has healed" + _hasHealed + " isAmmo?" + _isAmmo + "isground?" + _isGround);
        if (!_isGround)
        {
            //Debug.Log("NotGround");
            if (_hasHealed) return;
            if (!other.CompareTag("Player")) return;

            SpaceHealthHandler healthHandler = other.GetComponentInParent<SpaceHealthHandler>();
            if (healthHandler == null) return;
            if (healthHandler.Health >= healthHandler.MaxHealth) return;

            _hasHealed = true;
            healthHandler.Heal(_healAmount);
            Destroy(gameObject);
        }
        else if (_isGround && !_isAmmo)
        {
            //Debug.Log("Ground");
            if (_hasHealed) return;
            if (!other.CompareTag("Player")) return;
            if(other.gameObject.GetComponent<PlayerMovementAlienShooter>().currentHealth < 100)
            {
                other.gameObject.GetComponent<PlayerMovementAlienShooter>().currentHealth += 20;
                if(other.gameObject.GetComponent<PlayerMovementAlienShooter>().currentHealth > 100)
                {
                    other.gameObject.GetComponent<PlayerMovementAlienShooter>().currentHealth = 100;
                }
            }
            Destroy(gameObject);
        }
        else if(_isAmmo)
        {
            //Debug.Log("ammo");
            if (_hasHealed) return;
            if (!other.CompareTag("Player")) return;
            other.gameObject.GetComponentInChildren<RifleController>().reserveAmmo += 60;
            //Debug.Log("Ammod added??");
            Destroy(gameObject);
        }
        //Debug.Log(other.gameObject.name);
        
    }
}