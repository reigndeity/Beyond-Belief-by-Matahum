using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Collections;

public class Mangkukulam_MovementBehaviour : MonoBehaviour
{
    [Header("References")]
    private Mangkukulam_AttackManager atkMngr;
    private Mangkukulam_AnimationManager animator;

    public float wanderRadius = 10f;      // Radius to wander around the center
    public float wanderInterval = 3f;     // Time between picking new points

    private NavMeshAgent agent;
    private float timer;

    private Vector3 centerPoint;          // The center of the wandering area

    void Start()
    {
        atkMngr = GetComponent<Mangkukulam_AttackManager>();
        animator = GetComponent<Mangkukulam_AnimationManager>();

        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;

        // Set the center point to the spawn position
        centerPoint = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (atkMngr.isAttacking || Mangkukulam.instance.isDead || Mangkukulam.instance.isStunned)
        {
            agent.enabled = false;
            return;
        }
        else
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.updateRotation = true;

            if (HasReachedDestination())
                animator.ChangeAnimationState("Mangkukulam_Idle");
            else
                animator.ChangeAnimationState("Mangkukulam_Angry_Walk");
            
        }

        if (timer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(centerPoint, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private bool HasReachedDestination()
    {
        if (!agent.pathPending) // agent has a path
        {
            if (agent.remainingDistance <= agent.stoppingDistance) // close enough
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) // actually stopped
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Get a random point on the NavMesh inside a radius from a center point
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    // Draw Gizmo in Scene view to visualize the wander radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? centerPoint : transform.position, wanderRadius);
    }
}
