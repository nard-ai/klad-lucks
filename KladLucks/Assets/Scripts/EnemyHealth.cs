using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 3f;
    public float currentHealth;
    
    [Header("Visual Feedback")]
    public bool showDamageInConsole = true;
    
    void Start()
    {
        // Set health to maximum at start
        currentHealth = maxHealth;
        
        if (showDamageInConsole)
        {
            Debug.Log($"{gameObject.name} spawned with {currentHealth} health");
        }
    }
    
    public void TakeDamage(float damage)
    {
        // Reduce health
        currentHealth -= damage;
        
        if (showDamageInConsole)
        {
            Debug.Log($"{gameObject.name} takes {damage} damage! Health: {currentHealth}/{maxHealth}");
        }
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Visual feedback for taking damage (optional)
            ShowDamageEffect();
        }
    }
    
    void Die()
    {
        if (showDamageInConsole)
        {
            Debug.Log($"{gameObject.name} has been defeated!");
        }
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    void ShowDamageEffect()
    {
        // Simple damage effect - change color briefly
        StartCoroutine(FlashRed());
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        
        // Flash red
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        // Return to original color
        renderer.material.color = originalColor;
    }
}