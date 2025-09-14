using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool gameWon = false;
    public bool gameOver = false;
    public bool showVictoryInConsole = true;
    
    [Header("Victory Settings")]
    public float restartDelay = 3f;
    
    private float victoryTime;
    private float gameOverTime;
    
    void Update()
    {
        // Check ONLY if enemy base is destroyed for victory
        if (!gameWon && !gameOver)
        {
            // Victory condition: Enemy Base destroyed
            if (PlayerPrefs.GetInt("EnemyBaseDestroyed", 0) == 1)
            {
                Victory();
            }
            // Game Over condition: Player Base destroyed (handled by PlayerBase script)
        }
        
        // Handle restart after victory or game over
        if ((gameWon && Time.time - victoryTime >= restartDelay) || 
            (gameOver && Time.time - gameOverTime >= restartDelay))
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
    
    public void Victory()
    {
        gameWon = true;
        victoryTime = Time.time;
        
        if (showVictoryInConsole)
        {
            Debug.Log("🎉 VICTORY! All enemies defeated!");
            Debug.Log($"Press SPACE or R to restart in {restartDelay} seconds...");
        }
        
        // Stop all units (both old and new system)
        UnitMovement[] oldUnits = FindObjectsOfType<UnitMovement>();
        foreach (UnitMovement unit in oldUnits)
        {
            unit.enabled = false;
        }
        
        UnitBehavior[] newUnits = FindObjectsOfType<UnitBehavior>();
        foreach (UnitBehavior unit in newUnits)
        {
            unit.enabled = false;
        }
    }
    
    void RestartGame()
    {
        if (showVictoryInConsole)
        {
            Debug.Log("Restarting game...");
        }
        
        // Clear victory flags
        PlayerPrefs.SetInt("EnemyBaseDestroyed", 0);
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GameOver()
    {
        gameOver = true;
        gameOverTime = Time.time;
        
        if (showVictoryInConsole)
        {
            Debug.Log("💀 GAME OVER! Player Base destroyed!");
            Debug.Log($"Press SPACE or R to restart in {restartDelay} seconds...");
        }
        
        // Stop all units
        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();
        foreach (UnitMovement unit in allUnits)
        {
            unit.enabled = false;
        }
    }
    
    void OnGUI()
    {
        if (gameWon)
        {
            // Victory UI
            GUI.skin.label.fontSize = 30;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            GUI.Label(new Rect(0, screenHeight * 0.3f, screenWidth, 50), "🎉 VICTORY! 🎉");
            GUI.Label(new Rect(0, screenHeight * 0.4f, screenWidth, 30), "All enemies defeated!");
            
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(0, screenHeight * 0.6f, screenWidth, 30), "Press SPACE or R to restart");
        }
        else if (gameOver)
        {
            // Game Over UI
            GUI.skin.label.fontSize = 30;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            GUI.Label(new Rect(0, screenHeight * 0.3f, screenWidth, 50), "💀 GAME OVER 💀");
            GUI.Label(new Rect(0, screenHeight * 0.4f, screenWidth, 30), "Player Base destroyed!");
            
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(0, screenHeight * 0.6f, screenWidth, 30), "Press SPACE or R to restart");
        }
    }
}