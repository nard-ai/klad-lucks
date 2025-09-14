using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BattleCatsUI : MonoBehaviour
{
    [System.Serializable]
    public class UnitType
    {
        public string unitName;
        public GameObject unitPrefab;
        public int cost;
        public float cooldown;
        [HideInInspector] public float lastSpawnTime;
    }
    
    [Header("UI Settings")]
    public int startingMoney = 200;
    public int moneyPerSecond = 50;
    public Transform playerSpawnPoint;
    
    [Header("Available Units")]
    public List<UnitType> availableUnits = new List<UnitType>();
    
    // UI Elements
    private Canvas canvas;
    private Text moneyText;
    private int currentMoney;
    private List<Button> unitButtons = new List<Button>();
    
    void Start()
    {
        currentMoney = startingMoney;
        
        // Auto-find player spawn point if not assigned
        if (playerSpawnPoint == null)
        {
            GameObject playerBase = GameObject.Find("PlayerBase");
            if (playerBase != null)
            {
                playerSpawnPoint = playerBase.transform;
                Debug.Log("🐱 Auto-found PlayerBase for spawning!");
            }
        }
        
        // Auto-setup default units if none assigned
        if (availableUnits.Count == 0)
        {
            SetupDefaultUnits();
        }
        
        CreateUI();
        
        // Start money generation
        InvokeRepeating("GenerateMoney", 1f, 1f);
    }
    
    void SetupDefaultUnits()
    {
        // Try to get prefabs from BattleCatsSetup component
        BattleCatsSetup battleSetup = FindObjectOfType<BattleCatsSetup>();
        
        if (battleSetup != null)
        {
            if (battleSetup.basicCatPrefab != null)
            {
                UnitType basicUnit = new UnitType();
                basicUnit.unitName = "Basic Cat";
                basicUnit.unitPrefab = battleSetup.basicCatPrefab;
                basicUnit.cost = 75;
                basicUnit.cooldown = 2f;
                availableUnits.Add(basicUnit);
                Debug.Log("🐱 Added Basic Cat unit from BattleCatsSetup!");
            }
            
            if (battleSetup.tankCatPrefab != null)
            {
                UnitType tankUnit = new UnitType();
                tankUnit.unitName = "Tank Cat";
                tankUnit.unitPrefab = battleSetup.tankCatPrefab;
                tankUnit.cost = 150;
                tankUnit.cooldown = 5f;
                availableUnits.Add(tankUnit);
                Debug.Log("🐱 Added Tank Cat unit from BattleCatsSetup!");
            }
        }
        
        Debug.Log($"🎮 Setup {availableUnits.Count} default units!");
    }
    
    void CreateUI()
    {
        // Create EventSystem if it doesn't exist (required for UI interactions)
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("🎮 Created EventSystem for UI interactions!");
        }
        
        // Create Canvas
        GameObject canvasGO = new GameObject("BattleCatsCanvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        // Add CanvasScaler and GraphicRaycaster
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        
        CreateMoneyDisplay();
        CreateUnitButtons();
        
        Debug.Log("🎮 Battle Cats UI created successfully!");
    }
    
    void CreateMoneyDisplay()
    {
        // Money Display (Top Left)
        GameObject moneyGO = new GameObject("MoneyDisplay");
        moneyGO.transform.SetParent(canvas.transform, false);
        
        moneyText = moneyGO.AddComponent<Text>();
        moneyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        moneyText.fontSize = 36;
        moneyText.color = Color.yellow;
        moneyText.text = "💰 $" + currentMoney;
        
        // Position at top-left
        RectTransform moneyRect = moneyGO.GetComponent<RectTransform>();
        moneyRect.anchorMin = new Vector2(0, 1);
        moneyRect.anchorMax = new Vector2(0, 1);
        moneyRect.anchoredPosition = new Vector2(150, -50);
        moneyRect.sizeDelta = new Vector2(300, 50);
    }
    
    void CreateUnitButtons()
    {
        // Bottom Panel Background
        GameObject panelGO = new GameObject("BottomPanel");
        panelGO.transform.SetParent(canvas.transform, false);
        
        Image panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0.4f, 0.2f, 0.1f, 0.8f); // Brown Battle Cats style
        
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 0);
        panelRect.anchoredPosition = new Vector2(0, 75);
        panelRect.sizeDelta = new Vector2(0, 150);
        
        // Create buttons for each unit type
        for (int i = 0; i < availableUnits.Count; i++)
        {
            CreateUnitButton(availableUnits[i], i, panelGO.transform);
        }
    }
    
    void CreateUnitButton(UnitType unitType, int index, Transform parent)
    {
        // Button GameObject
        GameObject buttonGO = new GameObject(unitType.unitName + "Button");
        buttonGO.transform.SetParent(parent, false);
        
        // Button Component
        Button button = buttonGO.AddComponent<Button>();
        Image buttonImg = buttonGO.AddComponent<Image>();
        buttonImg.color = new Color(0.2f, 0.6f, 1f, 0.8f); // Blue button
        
        // Button positioning
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(120, 120);
        buttonRect.anchoredPosition = new Vector2(150 + (index * 140), 0);
        
        // Button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = unitType.unitName + "\n$" + unitType.cost;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Button click event
        button.onClick.AddListener(() => {
            Debug.Log($"🖱️ Button clicked: {unitType.unitName}");
            SpawnUnit(unitType, button);
        });
        unitButtons.Add(button);
        
        Debug.Log($"🐱 Created button for {unitType.unitName} - Cost: ${unitType.cost}");
    }
    
    void GenerateMoney()
    {
        currentMoney += moneyPerSecond;
        UpdateMoneyDisplay();
    }
    
    void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = "💰 $" + currentMoney;
        }
    }
    
    void SpawnUnit(UnitType unitType, Button button)
    {
        Debug.Log($"🐱 Attempting to spawn {unitType.unitName}...");
        
        // Check cooldown
        if (Time.time - unitType.lastSpawnTime < unitType.cooldown)
        {
            FlashButton(button, Color.blue); // Cooldown flash
            Debug.Log($"🐱 {unitType.unitName} is on cooldown!");
            return;
        }
        
        // Check money
        if (currentMoney < unitType.cost)
        {
            FlashButton(button, Color.red); // Not enough money flash
            Debug.Log($"🐱 Not enough money for {unitType.unitName}! Need ${unitType.cost}, have ${currentMoney}");
            return;
        }
        
        // Debug spawn point and prefab
        Debug.Log($"🐱 PlayerSpawnPoint: {(playerSpawnPoint != null ? playerSpawnPoint.name : "NULL")}");
        Debug.Log($"🐱 UnitPrefab: {(unitType.unitPrefab != null ? unitType.unitPrefab.name : "NULL")}");
        
        // Spawn the unit
        if (playerSpawnPoint != null && unitType.unitPrefab != null)
        {
            Vector3 spawnPos = playerSpawnPoint.position + new Vector3(1, 0, 0);
            Debug.Log($"🐱 Spawning at position: {spawnPos}");
            GameObject newUnit = Instantiate(unitType.unitPrefab, spawnPos, Quaternion.identity);
            
            // Deduct money
            currentMoney -= unitType.cost;
            unitType.lastSpawnTime = Time.time;
            
            UpdateMoneyDisplay();
            FlashButton(button, Color.green); // Success flash
            
            Debug.Log($"🐱 Successfully spawned {unitType.unitName} for ${unitType.cost}! Money remaining: ${currentMoney}");
        }
        else
        {
            Debug.LogError($"🚫 Cannot spawn {unitType.unitName}! PlayerSpawnPoint: {playerSpawnPoint}, Prefab: {unitType.unitPrefab}");
            FlashButton(button, Color.red);
        }
    }
    
    void FlashButton(Button button, Color flashColor)
    {
        // Simple color flash feedback
        Image buttonImg = button.GetComponent<Image>();
        Color originalColor = buttonImg.color;
        buttonImg.color = flashColor;
        
        // Reset color after 0.2 seconds
        Invoke("ResetButtonColor", 0.2f);
    }
    
    void ResetButtonColor()
    {
        // Reset all button colors
        foreach (Button btn in unitButtons)
        {
            btn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 0.8f);
        }
    }
}