using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    // ===================== ENUMS =====================
    enum EnemyState { Roaming, Chasing, Rotating, Laser, Dead }
    EnemyState currentState;

    // ===================== REFERENCES =====================
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public GameObject projectilesOrLaserParent, healthPrefab, ammoPrefab;

    // ===================== DETECTION =====================
    [Header("Detection")]
    public float sightRange = 15f;
    public float laserRange = 8f;
    public LayerMask playerLayer, enemyLayer;

    // ===================== HEALTH =====================
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead, isAttacked = false;

    // ===================== FIREBALL =====================
    [Header("Fireball Attack")]
    public GameObject fireballPrefab, fireball;
    public Transform firePoint;
    public float fireballCooldown = 3f;
    public float throwSpeed = 15f;
    public float maxRange = 25f;
    private float nextFireballTime;
    private bool isThrowingFireball = false;
    private Rigidbody fbRb;
    FireBallProjectile projectileScript;

    // ===================== LASER =====================
    [Header("Laser Attack")]
    public GameObject laserPrefab, laserTester;
    public Transform laserShootingPoint, rayCastLoc;
    private Vector3 playerPosAtShootingTime;
    public ParticleSystem particleSystemLaser, laserItself;
    private LineRenderer lr;
    public float laserCooldown = 10f, laserSweepSpeed = 40f;
    private float nextLaserTime = 10f;
    private bool isLaserGoing = false;
    Collider playerCollider;
    Vector3 extendedEnd;

    // ===================== MELEE =====================
    [Header("Melee Slam")]
    public float meleeRange = 3f;
    public float meleeCooldown = 4f;
    public float meleeDamage = 25f;
    public float meleeKnockback = 20f;
    private float nextMeleeTime = 5f;
    private bool meleeHitDealt = false;
    private bool isMeleeAttacking = false;
    public ParticleSystem shieldParticleSystem;

    // ===================== ROAMING =====================
    [Header("Roaming")]
    public float roamRadius = 15f;
    public float roamDelay = 3f;
    private float roamTimer;

    // ===================== EFFECTS =====================
    [Header("Effects/Sounds")]
    public ParticleSystem fireEffectForDamage;

    // ===================== TURN ANIMATION =====================
    [Header("Turn Animation")]
    public float turnAngleThreshold = 15f;
    private float previousYRotation;

    // ===================== START =====================
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        SnapToNavMesh();
        lr = particleSystemLaser.GetComponent<LineRenderer>();
        playerCollider = player.GetComponent<Collider>();
        previousYRotation = transform.eulerAngles.y;
        SetState(EnemyState.Roaming);
    }

    // ===================== UPDATE =====================
    void Update()
    {
        if (isDead) return;
        switch (currentState)
        {
            case EnemyState.Roaming: HandleRoaming(); break;
            case EnemyState.Chasing: HandleChasing(); break;
            case EnemyState.Rotating: HandleRotating(); break;
            case EnemyState.Laser: break;
            case EnemyState.Dead: break;
        }

        UpdateAnimations();
        UpdateTurnAnimation();
        particleSystemLaser.transform.parent.LookAt(extendedEnd);
        Debug.DrawRay(transform.position, transform.forward * 500f, Color.green);
        Debug.DrawLine(transform.position, playerPosAtShootingTime, Color.red);
    }

    // ===================== STATE MANAGEMENT =====================
    void SetState(EnemyState newState)
    {
        currentState = newState;
    }

    // ===================== ROAMING =====================
    void HandleRoaming()
    {
        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if (playerInSight || isAttacked)
        {
            SetState(EnemyState.Chasing);
            return;
        }

        roamTimer += Time.deltaTime;
        agent.isStopped = false;
        agent.updateRotation = true;

        if (roamTimer >= roamDelay && agent.remainingDistance < 1f)
        {
            Vector3 newPos = RandomNavSphere(transform.position, roamRadius);
            agent.SetDestination(newPos);
            roamTimer = 0f;
        }
    }
    

    bool IsAttacking()
    {
        return isMeleeAttacking || isThrowingFireball || isLaserGoing || currentState == EnemyState.Rotating || currentState == EnemyState.Laser;
    }
    // ===================== CHASING =====================
    void HandleChasing()
    {
        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!playerInSight && !isAttacked)
        {
            SetState(EnemyState.Roaming);
            return;
        }

        if (distanceToPlayer <= meleeRange && Time.time >= nextMeleeTime && !isMeleeAttacking)
        {
            if (isThrowingFireball)
            {
                isThrowingFireball = false;
                if (fireball != null) { Destroy(fireball); fireball = null; }
                animator.ResetTrigger("FireBall");
            }
            StartCoroutine(MeleeSlam());
            return;
        }
        if (IsAttacking()) return;

        // Laser attack
        if (distanceToPlayer <= laserRange && Time.time >= nextLaserTime)
        {
            playerPosAtShootingTime = player.position;
            StopAgent();
            SetState(EnemyState.Rotating);
            return;
        }

        if (distanceToPlayer <= laserRange)
        {
            agent.isStopped = true;
            agent.updateRotation = true;
            return;
        }

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.SetDestination(player.position);

        if (Time.time >= nextFireballTime && !isThrowingFireball)
            StartCoroutine(FireBall());
    }

    // ===================== ROTATING =====================
    void HandleRotating()
    {
        bool finishedRotating = RotateTowardsStoredPos();

        if (finishedRotating)
        {
            animator.SetTrigger("Laser");
            SetState(EnemyState.Laser);
        }
    }

    bool RotateTowardsStoredPos()
    {
        Vector3 dir = playerPosAtShootingTime - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return true;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            120f * Time.deltaTime
        );

        float dot = Vector3.Dot(transform.forward, dir.normalized);
        return dot > 0.995f;
    }

    // ===================== LASER =====================

    // Called by animation event
    public void ActivateShield()
    {
        Vector3 startPos = particleSystemLaser.transform.position;
        startPos += new Vector3(0, 0.12f, 0);
        Vector3 direction = (playerPosAtShootingTime - startPos).normalized;
        extendedEnd = playerPosAtShootingTime + direction * 10f;
        agent.isStopped = true;
        agent.ResetPath();
        StartCoroutine(LaserCoroutine());
    }

    // ===================== LASER =====================
    IEnumerator LaserCoroutine()
    {
        laserShootingPoint.gameObject.SetActive(true);
        particleSystemLaser.Play();
        laserItself.Play();
        animator.speed = 0f;

        var main = laserItself.main;
        main.startSizeY = 1500f;

        isLaserGoing = true;
        StartCoroutine(DamageThroughLaser());

        float elapsed = 0f;
        while (elapsed < 0.8f)
        {
            elapsed += Time.deltaTime;

            Vector3 lockedDir = playerPosAtShootingTime - transform.position;
            lockedDir.y = 0;
            if (lockedDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(lockedDir),
                    999f * Time.deltaTime
                );
            }

            Vector3 startPos = particleSystemLaser.transform.position + new Vector3(0, 0.12f, 0);
            extendedEnd = startPos + transform.forward * 20f;
            particleSystemLaser.transform.parent.LookAt(extendedEnd);
            yield return null;
        }
        float trackElapsed = 0f;
        while (trackElapsed < 2f)
        {
            trackElapsed += Time.deltaTime;

            Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    laserSweepSpeed * Time.deltaTime 
                );
            }

            Vector3 startPos = particleSystemLaser.transform.position + new Vector3(0, 0.12f, 0);
            Vector3 laserTarget = new Vector3(
                transform.position.x + transform.forward.x * 20f,
                player.position.y, 
                transform.position.z + transform.forward.z * 20f
            );
            extendedEnd = laserTarget;
            particleSystemLaser.transform.parent.LookAt(extendedEnd);

            yield return null;
        }

        isLaserGoing = false;
        nextLaserTime = Time.time + laserCooldown;
        laserShootingPoint.gameObject.SetActive(false);
        animator.speed = 1f;

        SetState(EnemyState.Chasing);
    }

    IEnumerator DamageThroughLaser()
    {
        while (isLaserGoing)
        {
            RaycastHit hit;

            if (Physics.Raycast(rayCastLoc.position, transform.forward, out hit, laserRange * 3f))
            {
                if (hit.transform.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(10f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // ===================== FIREBALL =====================

    IEnumerator FireBall()
    {
        isThrowingFireball = true;

        fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.transform.parent = firePoint;
        fbRb = fireball.GetComponent<Rigidbody>();
        projectileScript = fireball.GetComponent<FireBallProjectile>();
        projectileScript.owner = gameObject;
        projectileScript.rbOn = true;
        fbRb.isKinematic = true;

        yield return new WaitForSeconds(1f);

        animator.SetTrigger("FireBall");
    }

    // Called by animation event
    public void Shoot()
    {
        if (isDead) return;

        if (fireball != null)
        {
            fbRb.isKinematic = false;
            Vector3 velocity = CalculateLaunchVelocity(firePoint.position, player.position, throwSpeed);
            projectileScript.rbOn = false;
            fbRb.linearVelocity = velocity;
            projectileScript.isThrown = true;
        }

        nextFireballTime = Time.time + fireballCooldown;
        isThrowingFireball = false;
    }

    Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 target, float speed)
    {
        Vector3 direction = target - start;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);
        float distance = directionXZ.magnitude;
        float heightDifference = direction.y;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float time = distance / speed;
        float yVelocity = heightDifference / time + 0.5f * gravity * time;
        Vector3 velocity = directionXZ.normalized * speed;
        velocity.y = yVelocity;
        return velocity;
    }

    // ===================== MELEE =====================
    IEnumerator MeleeSlam()
    {
        if (isDead) yield break;

        isMeleeAttacking = true;
        nextMeleeTime = Time.time + meleeCooldown;
        meleeHitDealt = false;
        StopAgent();

        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);

        animator.SetTrigger("Meele");
        animator.ResetTrigger("FireBall");
        yield return new WaitForSeconds(2.2f);
        if (isDead) yield break;

        agent.enabled = true;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.updatePosition = true;
        animator.speed = 1f;
        isMeleeAttacking = false;

        SetState(EnemyState.Chasing);
        shieldParticleSystem.gameObject.SetActive(false);
    }

    public void StartAttack()
    {
        //Debug.Log("MeeleAttackAnim start");
        agent.updateRotation = false;
    }

    // Called by animation event at moment of impact
    public void EndAttack()
    {
        shieldParticleSystem.gameObject.SetActive(true);
        shieldParticleSystem.Play();
        agent.updateRotation = true;
        
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= meleeRange)
        {
            if (player.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(meleeDamage);

            if (player.TryGetComponent<CharacterController>(out var cc))
            {
                Vector3 knockDir = (player.position - transform.position).normalized;
                StartCoroutine(KnockbackPlayer(cc, knockDir));
            }
        }
    }

    IEnumerator KnockbackPlayer(CharacterController cc, Vector3 direction)
    {
        float elapsed = 0f;
        float knockDuration = 0.3f;

        while (elapsed < knockDuration)
        {
            elapsed += Time.deltaTime;
            cc.Move(direction * meleeKnockback * Time.deltaTime);
            yield return null;
        }
    }

    // ===================== HEALTH =====================
    public void TakeDamage(float amount, bool isPenetrating = false)
    {
        if (isDead) return;
        if (!isAttacked)
        {
            CallForBackup();
        }
        isAttacked = true;
        
        currentHealth -= amount;

        DamageNumberSpawner.Instance.Spawn(amount, transform.position + Vector3.up * 1.5f);

        if (currentHealth <= 20)
        {
            fireEffectForDamage.gameObject.SetActive(true);
            fireEffectForDamage.Play();
        }

        if (currentHealth <= 0)
            StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        SetState(EnemyState.Dead);
        isDead = true;

        isMeleeAttacking = false;
        isThrowingFireball = false;
        if (fireball != null) { Destroy(fireball); fireball = null; }
        animator.ResetTrigger("FireBall");

        agent.isStopped = true;
        agent.enabled = false;
        agent.ResetPath();
        agent.updatePosition = false;
        agent.updateRotation = false;
        animator.SetBool("Dead", true);

        yield return new WaitForSeconds(5f);
        Instantiate(ammoPrefab, gameObject.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        Instantiate(healthPrefab, gameObject.transform.position + new Vector3(2, 2, 0), Quaternion.identity);
        Destroy(gameObject);
    }

    // ===================== UTILS =====================
    void StopAgent()
    {
        agent.isStopped = true;
        agent.ResetPath();
        agent.updateRotation = false;
    }

    void UpdateAnimations()
    {
        animator.SetFloat("IdleOrWalk", agent.velocity.magnitude);
    }

    void UpdateTurnAnimation()
    {
        float currentYRotation = transform.eulerAngles.y;
        float delta = Mathf.DeltaAngle(previousYRotation, currentYRotation);
        previousYRotation = currentYRotation;

        bool isMoving = agent.velocity.magnitude > 0.1f;

        if (isMoving)
        {
            animator.SetBool("isTurning", false);
            animator.SetFloat("TurnAngle", 0f, 0.1f, Time.deltaTime);
            return;
        }

        float turnAngle = Mathf.Clamp(delta * 50f, -180f, 180f);
        animator.SetFloat("TurnAngle", turnAngle, 0.1f, Time.deltaTime);

        bool isTurning = Mathf.Abs(turnAngle) > turnAngleThreshold;
        animator.SetBool("isTurning", isTurning);
    }

    void SnapToNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            transform.position = hit.position;
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDir = Random.insideUnitSphere * distance + origin;
        NavMesh.SamplePosition(randomDir, out NavMeshHit hit, distance, NavMesh.AllAreas);
        return hit.position;
    }

    void CallForBackup()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, 15f, enemyLayer);

        foreach (Collider col in nearbyEnemies)
        {
            if (col.gameObject == gameObject) continue;

            EnemyAI nearby = col.GetComponent<EnemyAI>();
            if (nearby != null && !nearby.isDead)
            {
                nearby.isAttacked = true;
            }
        }
    }

    public void PlayStep()
    {
        // Later: play footstep sound here
    }
}