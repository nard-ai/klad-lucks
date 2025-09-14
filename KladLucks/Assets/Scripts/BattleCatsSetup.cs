using UnityEngine;

public class BattleCatsSetup : MonoBehaviour
{
    [Header("Battle Cats Layout")]
    public GameObject playerBase;
    public GameObject enemyBase;
    public GameObject playerUnit;
    public GameObject enemyUnit;
    
    [Header("Position Settings")]
    public float battlefieldWidth = 20f;
    public float groundLevel = 0f;
    
    void Start()
    {
        SetupBattleField();
    }
    
    void SetupBattleField()
    {
        // Position Player Base on the LEFT (like Battle Cats)
        if (playerBase != null)
        {
            playerBase.transform.position = new Vector3(-battlefieldWidth/2, groundLevel, 0);
            Debug.Log($"Player Base positioned at LEFT: {playerBase.transform.position}");
        }
        
        // Position Enemy Base on the RIGHT
        if (enemyBase != null)
        {
            enemyBase.transform.position = new Vector3(battlefieldWidth/2, groundLevel, 0);
            Debug.Log($"Enemy Base positioned at RIGHT: {enemyBase.transform.position}");
        }
        
        // Position Player Unit (starts near player base)
        if (playerUnit != null)
        {
            playerUnit.transform.position = new Vector3(-battlefieldWidth/2 + 3, groundLevel, 0);
            Debug.Log($"Player Unit starts at: {playerUnit.transform.position}");
        }
        
        // Position Enemy Unit (starts near enemy base)
        if (enemyUnit != null)
        {
            enemyUnit.transform.position = new Vector3(battlefieldWidth/2 - 3, groundLevel, 0);
            Debug.Log($"Enemy Unit starts at: {enemyUnit.transform.position}");
        }
        
        Debug.Log("🐱 Battle Cats battlefield setup complete!");
        Debug.Log("Player (BLUE) on LEFT, Enemy (RED) on RIGHT");
        Debug.Log("Units will fight in the middle, then attack opposing bases!");
    }
}