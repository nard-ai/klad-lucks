using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;  // Slightly slower than player units
    public GameObject target;
    
    [Header("Combat Settings")]
    public float attackRange = 1f;
    public float attackDamage = 2f;  // Enemies hit harder
    public float attackCooldown = 2f;  // But attack slower
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    
    void Start()
    {
        // Find PlayerBase automatically
        if (target == null)
        {
            target = GameObject.Find("PlayerBase");
        }
        
        if (target == null)
        {
            // If no PlayerBase, find any PlayerUnit as backup
            GameObject playerUnit = GameObject.Find("PlayerUnit");
            if (playerUnit != null)
            {
                target = playerUnit;
            }
        }
        
        if (showDebugInfo && target != null)
        {
            Debug.Log($"{gameObject.name} will attack {target.name}");
        }
    }
    
    void Update()
    {
        // Only move if we have a target and game isn't over
        if (target != null && !FindObjectOfType<GameManager>().gameOver)
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
            // Stop and attack
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
        // Check cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name} attacks {target.name} for {attackDamage} damage!");
            }
            
            // Try to damage PlayerBase
            PlayerBase playerBase = target.GetComponent<PlayerBase>();
            if (playerBase != null)
            {
                playerBase.TakeDamage(attackDamage);
            }
            else
            {
                // Try to damage PlayerUnit (if we add health later)
                EnemyHealth unitHealth = target.GetComponent<EnemyHealth>();
                if (unitHealth != null)
                {
                    unitHealth.TakeDamage(attackDamage);
                }
            }
        }
    }
}