using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway")]
    public float swayAmount = 0.02f;
    public float swaySmooth = 6f;
    public float maxSwayAmount = 0.06f;

    [Header("Tilt")]
    public float tiltAmount = 3f;
    public float tiltSmooth = 8f;

    [Header("Movement Bob")]
    public float bobFrequency = 2f;
    public float bobHorizontalAmount = 0.01f;
    public float bobVerticalAmount = 0.01f;
    public float bobSmooth = 10f;

    [Header("Idle Breathing")]
    public float breathFrequency = 1f;
    public float breathVerticalAmount = 0.002f;
    public float breathHorizontalAmount = 0.001f;

    [Header("Recoil")]
    public float recoilSmooth = 6f;
    public float recoilReturnSmooth = 4f;

    [Header("References")]
    public CharacterController characterController;
    public Camera playerCamera;
    public float normalFOV = 60f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float bobTimer = 0f;
    private float breathTimer = 0f;
    private Vector3 swayPos;
    private Vector3 bobPos;
    private Vector3 breathPos;
    private Vector3 recoilTargetPos;
    private Quaternion recoilTargetRot;
    private Vector3 currentRecoilPos;
    private Quaternion currentRecoilRot;
    private float currentTilt;
    private bool isAiming = false;
    private Vector3 aimTargetPosition;
    private float aimTargetFOV;
    public bool isReloading = false;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        currentRecoilRot = Quaternion.identity;
        aimTargetPosition = initialPosition;
        aimTargetFOV = normalFOV;
    }

    void Update()
    {
        HandleSway();
        HandleTilt();
        HandleBob();
        HandleBreathing();
        HandleRecoilReturn();
        ApplyAll();
    }

    void HandleSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float multiplier = isAiming ? 0.3f : 1f;
        float swayX = Mathf.Clamp(-mouseX * swayAmount * multiplier, -maxSwayAmount, maxSwayAmount);
        float swayY = Mathf.Clamp(-mouseY * swayAmount * multiplier, -maxSwayAmount, maxSwayAmount);
        swayPos = Vector3.Lerp(swayPos, new Vector3(swayX, swayY, 0f), Time.deltaTime * swaySmooth);
    }

    void HandleTilt()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float targetTilt = -mouseX * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmooth);
    }

    void HandleBob()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = characterController.isGrounded && (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float multiplier = isAiming ? 0.3f : 1f;
        float frequency = isRunning ? bobFrequency * 1.5f : bobFrequency;
        float vAmount = (isRunning ? bobVerticalAmount * 1.3f : bobVerticalAmount) * multiplier;
        float hAmount = (isRunning ? bobHorizontalAmount * 1.3f : bobHorizontalAmount) * multiplier;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * frequency;
            bobPos = Vector3.Lerp(bobPos, new Vector3(
                Mathf.Cos(bobTimer) * hAmount,
                -Mathf.Abs(Mathf.Sin(bobTimer)) * vAmount,
                0f), Time.deltaTime * bobSmooth);
        }
        else
        {
            bobTimer = 0f;
            bobPos = Vector3.Lerp(bobPos, Vector3.zero, Time.deltaTime * bobSmooth);
        }
    }

    void HandleBreathing()
    {
        if (isAiming)
        {
            breathPos = Vector3.Lerp(breathPos, Vector3.zero, Time.deltaTime * 5f);
            return;
        }
        breathTimer += Time.deltaTime * breathFrequency;
        breathPos = new Vector3(
            Mathf.Sin(breathTimer * 0.7f) * breathHorizontalAmount,
            Mathf.Sin(breathTimer) * breathVerticalAmount,
            0f);
    }

    void HandleRecoilReturn()
    {
        recoilTargetPos = Vector3.Lerp(recoilTargetPos, Vector3.zero, Time.deltaTime * recoilReturnSmooth);
        recoilTargetRot = Quaternion.Lerp(recoilTargetRot, Quaternion.identity, Time.deltaTime * recoilReturnSmooth);
        currentRecoilPos = Vector3.Lerp(currentRecoilPos, recoilTargetPos, Time.deltaTime * recoilSmooth);
        currentRecoilRot = Quaternion.Lerp(currentRecoilRot, recoilTargetRot, Time.deltaTime * recoilSmooth);
    }

    void ApplyAll()
    {
        if (isReloading) return;
        Vector3 targetPos = isAiming ? aimTargetPosition : initialPosition;
        targetPos += swayPos + bobPos + breathPos + currentRecoilPos;
        Quaternion tiltRot = Quaternion.Euler(0f, 0f, currentTilt);
        Quaternion finalRot = initialRotation * tiltRot * currentRecoilRot;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, aimTargetFOV, Time.deltaTime * 8f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * bobSmooth);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRot, Time.deltaTime * bobSmooth);
    }

    public void TriggerRecoil(float kickBack, float kickUp, float rotationUp)
    {
        if (isAiming)
        {
            kickUp = 0;
            rotationUp = 0;
        }
        recoilTargetPos += new Vector3(0f, kickUp, -kickBack);
        recoilTargetRot *= Quaternion.Euler(-rotationUp, 0f, 0f);
    }

    public void SetAim(bool aiming, Vector3 aimPosition, float fov)
    {
        //Debug.Log("is aiming");

        isAiming = aiming;
        aimTargetPosition = aiming ? aimPosition : initialPosition;
        aimTargetFOV = aiming ? fov : normalFOV;
    }
}