using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Settings")]
    public float maxHealth = 10f;
    public float currentHealth;
    
    [Header("Visual Feedback")]
    public bool showDamageInConsole = true;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (showDamageInConsole)
        {
            Debug.Log($"Enemy Base initialized with {currentHealth} health");
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (showDamageInConsole)
        {
            Debug.Log($"Enemy Base takes {damage} damage! Health: {currentHealth}/{maxHealth}");
        }
        
        if (currentHealth <= 0)
        {
            BaseDestroyed();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }
    
    void BaseDestroyed()
    {
        if (showDamageInConsole)
        {
            Debug.Log("🎉 Enemy Base destroyed! Player Victory!");
        }
        
        // Trigger victory by setting a flag that GameManager can check
        PlayerPrefs.SetInt("EnemyBaseDestroyed", 1);
        
        // Destroy the base
        Destroy(gameObject);
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
        }
    }
}