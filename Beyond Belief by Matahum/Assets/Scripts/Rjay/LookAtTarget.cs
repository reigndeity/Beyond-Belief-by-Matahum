using UnityEngine;
using FIMSpace.FLook; // Update if you're using a different namespace

public class LookAtTarget : MonoBehaviour
{
    public FLookAnimator lookAnimator;
    public float detectionRadius = 10f;
    public LayerMask targetLayer; // Set this in the Inspector

    void Awake()
    {
        lookAnimator = GetComponent<FLookAnimator>();
    }
    void Update()
    {
        Transform closest = GetClosestTarget();
        lookAnimator.ObjectToFollow = closest;
    }

    Transform GetClosestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(currentPosition, col.transform.position);
            if (distance < minDistance)
            {
                closest = col.transform;
                minDistance = distance;
            }
        }

        return closest;
    }
}
