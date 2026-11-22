using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementAlienShooter : MonoBehaviour, IDamageable
{
    public Camera playerCamera;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpPower = 4f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    [Header("References")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject _camera, damageTakenPanel;

    [Header("Audio")]
    AudioSource _audioSource;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Shield")]
    public float maxShield = 100f;
    public float currentShield;
    public float shieldActiveCost = 75f;
    public float shieldRechargeRate = 10f;
    public float sprintDrainRate = 5f;
    public float maxPenetratingDamage = 100f;
    public ParticleSystem shieldParticle;
    private bool shieldActive = false;
    public GameObject shieldIcon;

    [Header("UI Gauges")]
    public Image healthFill;
    public Image shieldFill;
    public float uiLerpSpeed = 5f;

    [Header("Weapon")]
    public WeaponHolder weaponHolder;

    private float healthTarget;
    private float shieldTarget;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true, isRunning;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        _audioSource.Play();
        currentHealth = maxHealth;
        currentShield = maxShield;
        healthTarget = currentHealth;
        shieldTarget = currentShield;
        weaponHolder.EquipWeapon(0);
    }

    void Update()
    {
        if (currentShield >= 75f)
        {
            shieldIcon.SetActive(true); 
        }
        else
        {
            shieldIcon.SetActive(false);
        }
        if (Time.timeScale == 0) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (currentShield > 7)
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }  
        }
        else
        {
            isRunning = false;
        }

        if(currentHealth <= 0)
        {
            PlayerDead();
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            moveDirection.y = jumpPower;
        else
            moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 3f;
            runSpeed = 6f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Weapon input
        if (Input.GetMouseButton(0))
            weaponHolder.currentWeapon?.Shoot();

        if (Input.GetKeyDown(KeyCode.R))
            weaponHolder.currentWeapon?.Reload();

        if (Input.GetMouseButton(1))
        {
            weaponHolder.currentWeapon?.Aim(true);
            //Debug.Log("playerAim");
        }
        else
        {
            weaponHolder.currentWeapon?.Aim(false);
        }
        //weaponHolder.currentWeapon?.Aim(Input.GetMouseButton(1));


        // Weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1)) weaponHolder.EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) weaponHolder.EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) weaponHolder.EquipWeapon(2);

        if (!shieldActive && currentShield < maxShield && !isRunning)
        {
            currentShield += shieldRechargeRate * Time.deltaTime;
            currentShield = Mathf.Min(currentShield, maxShield);
        }

        if (isRunning)
        {
            currentShield -= sprintDrainRate * Time.deltaTime;
            currentShield = Mathf.Max(currentShield, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentShield >= maxShield * 0.75f)
            {
                shieldActive = true;
                currentShield -= shieldActiveCost;
                shieldParticle.gameObject.SetActive(true);
                shieldParticle.Play();
            }
            else
            {
                Debug.Log("Not enough shield!");
            }
        }

        // UI
        healthTarget = currentHealth / maxHealth;
        shieldTarget = currentShield / maxShield;
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, healthTarget, Time.deltaTime * uiLerpSpeed);
        shieldFill.fillAmount = Mathf.Lerp(shieldFill.fillAmount, shieldTarget, Time.deltaTime * uiLerpSpeed);
    }

    public void TakeDamage(float damage, bool isPenetrating = false)
    {
        if (shieldActive)
        {
            if (isPenetrating)
            {
                //Debug.Log("ShieldBlockedAttack");
                float blocked = Mathf.Min(damage, maxPenetratingDamage);
                float remaining = damage - blocked;
                currentHealth -= remaining;
                currentHealth = Mathf.Max(currentHealth, 0f);
            }

            shieldActive = false;
            shieldParticle.Stop();
            shieldParticle.gameObject.SetActive(false);
            //Debug.Log("ShieldGone");
            return;
        }
        StartCoroutine(DamageEffect());
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
    }

    private void PlayerDead()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("DeathSceneGround");
    }

    public IEnumerator DamageEffect()
    {
        damageTakenPanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damageTakenPanel.SetActive(false);
    }
}