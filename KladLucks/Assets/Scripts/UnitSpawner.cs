using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public bool isPlayerSpawner = true;
    
    [Header("AI Enemy Spawning")]
    public bool enableAISpawning = true;
    public float aiSpawnInterval = 5f;
    public GameObject[] aiUnitPrefabs;
    public int aiSpawnCost = 100;
    
    private float lastAISpawnTime;
    private BattleCatsUI uiManager;
    
    void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
            
        uiManager = FindObjectOfType<BattleCatsUI>();
        lastAISpawnTime = Time.time;
    }
    
    void Update()
    {
        // AI Enemy Spawning (automatic)
        if (!isPlayerSpawner && enableAISpawning)
        {
            if (Time.time - lastAISpawnTime >= aiSpawnInterval)
            {
                SpawnAIUnit();
                lastAISpawnTime = Time.time;
            }
        }
    }
    
    void SpawnAIUnit()
    {
        if (aiUnitPrefabs.Length == 0) return;
        
        // Randomly select a unit type
        GameObject unitToSpawn = aiUnitPrefabs[Random.Range(0, aiUnitPrefabs.Length)];
        
        // Spawn position with slight randomization
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y += Random.Range(-0.5f, 0.5f);
        
        GameObject newUnit = Instantiate(unitToSpawn, spawnPos, Quaternion.identity);
        
        // Setup unit behavior
        UnitBehavior unitBehavior = newUnit.GetComponent<UnitBehavior>();
        if (unitBehavior != null)
        {
            unitBehavior.isPlayerUnit = false; // This is an enemy unit
            Debug.Log($"AI spawned enemy unit at {spawnPos}");
        }
    }
    
    public void SpawnPlayerUnit(GameObject unitPrefab)
    {
        if (!isPlayerSpawner) return;
        
        Vector3 spawnPos = spawnPoint.position;
        GameObject newUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        
        UnitBehavior unitBehavior = newUnit.GetComponent<UnitBehavior>();
        if (unitBehavior != null)
        {
            unitBehavior.isPlayerUnit = true;
            Debug.Log($"Player spawned unit at {spawnPos}");
        }
    }
}