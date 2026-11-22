using System.Collections;
using UnityEngine;

public class RifleController : MonoBehaviour, IWeapon
{
    public string WeaponName => "Rifle";

    [Header("Shooting")]
    public float damage = 10f;
    public float range = Mathf.Infinity;
    public float fireRate = 0.1f, BulletSpeed = 100;
    public Camera playerCamera;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;
    public float reloadTime = 2f;
    private bool isReloading = false;
    public TMPro.TextMeshProUGUI ammoText;

    [Header("Bullet Spread")]
    public float hipSpread = 0.05f;
    public float aimSpread = 0.001f;

    [Header("Spread Buildup")]
    public float maxSpreadBuildup = 0.1f;
    public float spreadBuildupRate = 0.01f;
    public float spreadRecoveryRate = 0.05f;
    private float currentSpreadBuildup = 0f;

    [Header("Recoil")]
    public float recoilKickBack = 0.02f;
    public float recoilKickUp = 0.015f;
    public float recoilRotationUp = 1.5f;

    [Header("Aim")]
    public Vector3 aimPosition = new Vector3(0f, -0.05f, 0.1f);
    public float aimFOV = 40f;

    [Header("Reload Animation")]
    public float reloadTiltX = 40f;   
    public float reloadTiltZ = -20f;   
    public float reloadTiltY = 15f;    
    public Vector3 reloadOffset = new Vector3(0.1f, -0.3f, 0f); 

    [Header("References")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip reloadSound;
    public WeaponSway weaponSway;
    public TrailRenderer trailBullet;
    public Transform BulletSpawnPoint;

    private float nextFireTime = 0f;
    private bool isAiming = false;
    private Vector3 reloadStartPos;
    private Quaternion reloadStartRot;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (Time.time > nextFireTime)
        {
            currentSpreadBuildup = Mathf.Max(0f, currentSpreadBuildup - spreadRecoveryRate * Time.deltaTime);
        }

        if (currentAmmo <= 0 && !isReloading && reserveAmmo > 0)
        {
            StartCoroutine(DoReload());
        }

        ammoText.text = currentAmmo + " / " + reserveAmmo;
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        muzzleFlash?.Play();
        gunAudio?.Play();
        weaponSway?.TriggerRecoil(recoilKickBack, recoilKickUp, recoilRotationUp);

        currentSpreadBuildup = Mathf.Min(currentSpreadBuildup + spreadBuildupRate, maxSpreadBuildup);
        float spread = (isAiming ? aimSpread : hipSpread) + currentSpreadBuildup;

        Vector3 direction = playerCamera.transform.forward;
        direction += new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, direction, out hit, range))
        {
            TrailRenderer trail = Instantiate(trailBullet, BulletSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

            if (hit.transform.CompareTag("Enemy"))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(damage);
            }
            else if (hit.transform.CompareTag("Explodable"))
            {
                if (hit.collider.TryGetComponent<IExplodeable>(out var explodeable))
                    explodeable.Explode();
            }
        }
    }

    public void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo && reserveAmmo > 0)
            StartCoroutine(DoReload());
    }

    IEnumerator DoReload()
    {
        isReloading = true;
        weaponSway.isReloading = true;
        if (reloadSound != null)
            gunAudio?.PlayOneShot(reloadSound);

        reloadStartPos = transform.localPosition;
        reloadStartRot = transform.localRotation;

        yield return StartCoroutine(AnimateReload());

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);
        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;
        weaponSway.isReloading = false;
        isReloading = false;
        Debug.Log("Reloaded! Ammo: " + currentAmmo + " / " + reserveAmmo);
    }

    IEnumerator AnimateReload()
    {
        Debug.Log("Reload animation started");
        Vector3 downPos = reloadStartPos + reloadOffset;
        Quaternion downRot = reloadStartRot * Quaternion.Euler(reloadTiltX, reloadTiltY, reloadTiltZ);

        float phase1 = reloadTime * 0.35f; 
        float phase2 = reloadTime * 0.3f;  
        float phase3 = reloadTime * 0.35f; 
        float elapsed = 0f;
        while (elapsed < phase1)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / phase1);
            transform.localPosition = Vector3.Lerp(reloadStartPos, downPos, t);
            transform.localRotation = Quaternion.Lerp(reloadStartRot, downRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(phase2);

        elapsed = 0f;
        Vector3 currentPos = transform.localPosition;
        Quaternion currentRot = transform.localRotation;
        Debug.Log("Start: " + reloadStartPos + " Target: " + downPos);
        while (elapsed < phase3)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / phase3);
            transform.localPosition = Vector3.Lerp(currentPos, reloadStartPos, t);
            transform.localRotation = Quaternion.Lerp(currentRot, reloadStartRot, t);
            Debug.Log("Animating: " + transform.localPosition);
            yield return null;
        }

        transform.localPosition = reloadStartPos;
        transform.localRotation = reloadStartRot;
    }

    public void Aim(bool isAiming)
    {
        this.isAiming = isAiming;
        weaponSway?.SetAim(isAiming, aimPosition, aimFOV);
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= BulletSpeed * Time.deltaTime;
            yield return null;
        }

        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}