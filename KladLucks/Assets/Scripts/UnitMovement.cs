using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public GameObject target;
    
    [Header("Combat Settings")]
    public float attackRange = 1f;
    public float attackDamage = 1f;
    public float attackCooldown = 1f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private float lastAttackTime = 0f;
    private bool isAttacking = false;    void Start()
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
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        
        // Check if in attack range
        if (distanceToTarget <= attackRange)
        {
            // Stop moving and start attacking
            isAttacking = true;
            AttackTarget();
        }
        else
        {
            // Move toward target
            isAttacking = false;
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    
    void AttackTarget()
    {
        // Check if enough time has passed since last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name} attacks {target.name} for {attackDamage} damage!");
            }
            
            // Try to damage the target
            EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
            else
            {
                // If no health component, destroy immediately (simple version)
                if (showDebugInfo)
                {
                    Debug.Log($"{target.name} destroyed!");
                }
                Destroy(target);
                target = null; // Clear target so unit stops
            }
        }
    }
}