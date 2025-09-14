using UnityEngine;

public enum UnitState
{
    MOVING_TO_BASE,
    IN_COMBAT,
    ATTACKING_BASE,
    IDLE
}

public enum UnitType
{
    PLAYER_UNIT,
    ENEMY_UNIT
}

public class UnitBehavior : MonoBehaviour
{
    [Header("Unit Settings")]
    public UnitType unitType = UnitType.PLAYER_UNIT;
    public bool isPlayerUnit = true; // For easy Battle Cats UI integration
    public float moveSpeed = 2f;
    public float health = 3f;
    public float maxHealth = 3f;
    
    [Header("Combat Settings")]
    public float attackRange = 1f;
    public float attackDamage = 1f;
    public float attackCooldown = 1f;
    public float combatDetectionRange = 1.5f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // State Management
    public UnitState currentState = UnitState.MOVING_TO_BASE;
    private GameObject targetBase;
    private GameObject combatTarget;
    private float lastAttackTime = 0f;
    
    void Start()
    {
        maxHealth = health;
        FindTargetBase();
        
        if (showDebugInfo)
        {
            string baseTarget = targetBase ? targetBase.name : "No base found";
            Debug.Log($"{gameObject.name} ({unitType}) targeting: {baseTarget}");
        }
    }
    
    void Update()
    {
        // Skip updates if game is over
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager && (gameManager.gameWon || gameManager.gameOver))
            return;
            
        UpdateBehavior();
    }
    
    void UpdateBehavior()
    {
        switch (currentState)
        {
            case UnitState.MOVING_TO_BASE:
                HandleMovingToBase();
                break;
                
            case UnitState.IN_COMBAT:
                HandleCombat();
                break;
                
            case UnitState.ATTACKING_BASE:
                HandleAttackingBase();
                break;
                
            case UnitState.IDLE:
                HandleIdle();
                break;
        }
    }
    
    void HandleMovingToBase()
    {
        // Check for enemy units to fight
        GameObject nearbyEnemy = FindNearbyEnemy();
        if (nearbyEnemy != null)
        {
            EnterCombat(nearbyEnemy);
            return;
        }
        
        // Move toward target base
        if (targetBase != null)
        {
            float distanceToBase = Vector3.Distance(transform.position, targetBase.transform.position);
            
            if (distanceToBase <= attackRange)
            {
                // Reached base - start attacking
                ChangeState(UnitState.ATTACKING_BASE);
                if (showDebugInfo)
                {
                    Debug.Log($"{gameObject.name} reached {targetBase.name} - starting base attack!");
                }
            }
            else
            {
                // Battle Cats style movement - ONLY on X-axis (left/right)
                Vector3 direction = (targetBase.transform.position - transform.position).normalized;
                
                // Lock to X-axis only (2D side-scrolling like Battle Cats)
                direction.y = 0;
                direction.z = 0;
                direction = direction.normalized;
                
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            ChangeState(UnitState.IDLE);
        }
    }
    
    void HandleCombat()
    {
        if (combatTarget == null)
        {
            // Combat target destroyed - resume mission
            ChangeState(UnitState.MOVING_TO_BASE);
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name} won combat - resuming mission to {targetBase?.name}");
            }
            return;
        }
        
        float distanceToEnemy = Vector3.Distance(transform.position, combatTarget.transform.position);
        
        if (distanceToEnemy > combatDetectionRange)
        {
            // Enemy moved away - resume base mission
            combatTarget = null;
            ChangeState(UnitState.MOVING_TO_BASE);
            return;
        }
        
        if (distanceToEnemy <= attackRange)
        {
            // Attack enemy
            AttackTarget(combatTarget);
        }
        else
        {
            // Move closer to enemy
            Vector3 direction = (combatTarget.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    
    void HandleAttackingBase()
    {
        // Check for enemies that might interrupt base attack
        GameObject nearbyEnemy = FindNearbyEnemy();
        if (nearbyEnemy != null)
        {
            EnterCombat(nearbyEnemy);
            return;
        }
        
        // Attack the base
        if (targetBase != null)
        {
            AttackTarget(targetBase);
        }
        else
        {
            ChangeState(UnitState.IDLE);
        }
    }
    
    void HandleIdle()
    {
        // Look for new targets
        FindTargetBase();
        if (targetBase != null)
        {
            ChangeState(UnitState.MOVING_TO_BASE);
        }
    }
    
    void FindTargetBase()
    {
        if (unitType == UnitType.PLAYER_UNIT)
        {
            // Player units target Enemy Base
            targetBase = GameObject.Find("EnemyBase");
        }
        else // ENEMY_UNIT
        {
            // Enemy units target Player Base
            targetBase = GameObject.Find("PlayerBase");
        }
    }
    
    GameObject FindNearbyEnemy()
    {
        UnitBehavior[] allUnits = FindObjectsOfType<UnitBehavior>();
        
        foreach (UnitBehavior otherUnit in allUnits)
        {
            if (otherUnit == this) continue;
            if (otherUnit.unitType == this.unitType) continue; // Same faction
            
            float distance = Vector3.Distance(transform.position, otherUnit.transform.position);
            if (distance <= combatDetectionRange)
            {
                return otherUnit.gameObject;
            }
        }
        
        return null;
    }
    
    void EnterCombat(GameObject enemy)
    {
        combatTarget = enemy;
        ChangeState(UnitState.IN_COMBAT);
        
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name} entering combat with {enemy.name}!");
        }
    }
    
    void AttackTarget(GameObject target)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name} attacks {target.name} for {attackDamage} damage!");
            }
            
            // Try different damage methods
            UnitBehavior targetUnit = target.GetComponent<UnitBehavior>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(attackDamage);
            }
            else
            {
                // Try base components
                PlayerBase playerBase = target.GetComponent<PlayerBase>();
                if (playerBase != null)
                {
                    playerBase.TakeDamage(attackDamage);
                    return;
                }
                
                EnemyBase enemyBase = target.GetComponent<EnemyBase>();
                if (enemyBase != null)
                {
                    enemyBase.TakeDamage(attackDamage);
                    return;
                }
                
                EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name} takes {damage} damage! Health: {health}/{maxHealth}");
        }
        
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }
    
    void Die()
    {
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name} has been defeated!");
        }
        
        Destroy(gameObject);
    }
    
    void ChangeState(UnitState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name} changed to state: {newState}");
            }
        }
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = originalColor;
        }
    }
}