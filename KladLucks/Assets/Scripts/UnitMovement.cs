using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public GameObject target;

    [Header("Debug")]
    public bool showDebugInfo = true;

    void Start()
    {
        // Find the enemy automatically if no target is set
        if (target == null)
        {
            target = GameObject.Find("Enemy");
        }

        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name} will move toward {target.name}");
        }
    }

    void Update()
    {
        // Only move if we have a target
        if (target != null)
        {
            MoveTowardTarget();
        }
    }

    void MoveTowardTarget()
    {
        // Calculate direction from this unit to target
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // Move toward target
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Stop when close to target (optional - prevents jittering)
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget < 0.5f)
        {
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name} reached {target.name}!");
            }
        }
    }
}