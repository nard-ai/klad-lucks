using UnityEngine;

public class PlayerBase : MonoBehaviour
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
            Debug.Log($"Player Base initialized with {currentHealth} health");
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (showDamageInConsole)
        {
            Debug.Log($"Player Base takes {damage} damage! Health: {currentHealth}/{maxHealth}");
        }
        
        if (currentHealth <= 0)
        {
            GameOver();
        }
        else
        {
            // Visual feedback
            StartCoroutine(FlashRed());
        }
    }
    
    void GameOver()
    {
        if (showDamageInConsole)
        {
            Debug.Log("💀 GAME OVER! Player Base destroyed!");
        }
        
        // Find GameManager and trigger game over
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        renderer.material.color = originalColor;
    }
}