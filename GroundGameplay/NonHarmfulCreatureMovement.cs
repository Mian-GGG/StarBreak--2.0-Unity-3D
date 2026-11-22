using UnityEngine;
using UnityEngine.AI;

public class NonHarmfulCreatureMovement : MonoBehaviour
{
    [Header("Roaming")]
    public float roamRadius = 15f;
    public float roamDelay = 3f;
    private float roamTimer;
    NavMeshAgent agent;
    private bool isAttacked = false, isDead = false;
    Animator animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(!isDead && !isAttacked)
        {
            Roam();
            animator.SetFloat("IdleOrWalk",    agent.velocity.magnitude);
        }
    }

    void Roam()
    {
        roamTimer += Time.deltaTime;

        if (roamTimer >= roamDelay && agent.remainingDistance < 1f)
        {
            Vector3 newPos = RandomNavSphere(transform.position, roamRadius);
            agent.SetDestination(newPos);
            roamTimer = 0f;
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDir = Random.insideUnitSphere * distance + origin;
        NavMesh.SamplePosition(randomDir, out NavMeshHit hit, distance, NavMesh.AllAreas);
        return hit.position;

    }

    public void PlayStep()
    {
        // Later: play footstep sound here
        //Debug.Log("Footstep");
    }

    public void Grunt()
    {
        //something something
    }

}
