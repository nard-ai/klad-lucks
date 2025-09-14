using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool gameWon = false;
    public bool showVictoryInConsole = true;
    
    [Header("Victory Settings")]
    public float restartDelay = 3f;
    
    private float victoryTime;
    
    void Update()
    {
        // Check if all enemies are defeated
        if (!gameWon && GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            Victory();
        }
        
        // Handle restart after victory
        if (gameWon && Time.time - victoryTime >= restartDelay)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
    
    void Victory()
    {
        gameWon = true;
        victoryTime = Time.time;
        
        if (showVictoryInConsole)
        {
            Debug.Log("🎉 VICTORY! All enemies defeated!");
            Debug.Log($"Press SPACE or R to restart in {restartDelay} seconds...");
        }
        
        // Stop all player units
        UnitMovement[] playerUnits = FindObjectsOfType<UnitMovement>();
        foreach (UnitMovement unit in playerUnits)
        {
            unit.enabled = false; // Disable movement
        }
    }
    
    void RestartGame()
    {
        if (showVictoryInConsole)
        {
            Debug.Log("Restarting game...");
        }
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void OnGUI()
    {
        if (gameWon)
        {
            // Simple UI for victory
            GUI.skin.label.fontSize = 30;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Victory message
            GUI.Label(new Rect(0, screenHeight * 0.3f, screenWidth, 50), "🎉 VICTORY! 🎉");
            GUI.Label(new Rect(0, screenHeight * 0.4f, screenWidth, 30), "All enemies defeated!");
            
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(0, screenHeight * 0.6f, screenWidth, 30), "Press SPACE or R to restart");
        }
    }
}