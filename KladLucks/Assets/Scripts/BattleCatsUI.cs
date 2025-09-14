using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleCatsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Canvas gameCanvas;
    public GameObject unitButtonPrefab;
    public Transform bottomPanel;
    
    [Header("Resources")]
    public int playerMoney = 1000;
    public Text moneyDisplay;
    public int moneyPerSecond = 50;
    
    [Header("Unit Spawning")]
    public Transform playerSpawnPoint;
    public List<UnitType> availableUnits = new List<UnitType>();
    
    private float lastMoneyTime;
    
    [System.Serializable]
    public class UnitType
    {
        public string unitName;
        public GameObject unitPrefab;
        public int cost;
        public float cooldown;
        public Sprite icon;
        [HideInInspector] public float lastSpawnTime;
    }
    
    void Start()
    {
        SetupUI();
        CreateUnitButtons();
        lastMoneyTime = Time.time;
    }
    
    void Update()
    {
        // Generate money over time (like Battle Cats)
        if (Time.time - lastMoneyTime >= 1f)
        {
            playerMoney += moneyPerSecond;
            lastMoneyTime = Time.time;
            UpdateMoneyDisplay();
        }
    }
    
    void SetupUI()
    {
        // Create Canvas if not assigned
        if (gameCanvas == null)
        {
            GameObject canvasGO = new GameObject("BattleCatsCanvas");
            gameCanvas = canvasGO.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameCanvas.sortingOrder = 100;
            
            // Add CanvasScaler for responsive UI
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create bottom panel (like Battle Cats unit selection)
        if (bottomPanel == null)
        {
            GameObject panelGO = new GameObject("BottomPanel");
            panelGO.transform.SetParent(gameCanvas.transform, false);
            
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.15f, 0.1f, 0.9f); // Brown Battle Cats style
            
            RectTransform panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(1, 0);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(0, 150); // Height of bottom panel
            
            bottomPanel = panelGO.transform;
            
            // Add horizontal layout for unit buttons
            HorizontalLayoutGroup layout = panelGO.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(20, 20, 10, 10);
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }
        
        // Create money display
        if (moneyDisplay == null)
        {
            GameObject moneyGO = new GameObject("MoneyDisplay");
            moneyGO.transform.SetParent(gameCanvas.transform, false);
            
            moneyDisplay = moneyGO.AddComponent<Text>();
            moneyDisplay.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            moneyDisplay.fontSize = 36;
            moneyDisplay.color = Color.yellow;
            moneyDisplay.text = $"💰 {playerMoney}";
            moneyDisplay.alignment = TextAnchor.MiddleLeft;
            
            RectTransform moneyRect = moneyGO.GetComponent<RectTransform>();
            moneyRect.anchorMin = new Vector2(0, 1);
            moneyRect.anchorMax = new Vector2(0, 1);
            moneyRect.anchoredPosition = new Vector2(20, -30);
            moneyRect.sizeDelta = new Vector2(300, 50);
        }
        
        UpdateMoneyDisplay();
    }
    
    void CreateUnitButtons()
    {
        // Create buttons for each available unit type
        foreach (UnitType unitType in availableUnits)
        {
            CreateUnitButton(unitType);
        }
    }
    
    void CreateUnitButton(UnitType unitType)
    {
        GameObject buttonGO = new GameObject($"{unitType.unitName}Button");
        buttonGO.transform.SetParent(bottomPanel, false);
        
        // Button background
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.8f, 0.6f, 0.4f, 1f); // Light brown
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(120, 120);
        
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Unit icon
        if (unitType.icon != null)
        {
            GameObject iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(buttonGO.transform, false);
            
            Image iconImage = iconGO.AddComponent<Image>();
            iconImage.sprite = unitType.icon;
            
            RectTransform iconRect = iconGO.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(10, 25);
            iconRect.offsetMax = new Vector2(-10, -10);
        }
        
        // Cost text
        GameObject costGO = new GameObject("Cost");
        costGO.transform.SetParent(buttonGO.transform, false);
        
        Text costText = costGO.AddComponent<Text>();
        costText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        costText.fontSize = 20;
        costText.color = Color.white;
        costText.text = $"${unitType.cost}";
        costText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform costRect = costGO.GetComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0, 0);
        costRect.anchorMax = new Vector2(1, 0);
        costRect.anchoredPosition = new Vector2(0, 12);
        costRect.sizeDelta = new Vector2(0, 25);
        
        // Button click event
        button.onClick.AddListener(() => SpawnUnit(unitType, button));
        
        Debug.Log($"Created unit button for {unitType.unitName} - Cost: ${unitType.cost}");
    }
    
    public void SpawnUnit(UnitType unitType, Button button)
    {
        // Check if player has enough money
        if (playerMoney < unitType.cost)
        {
            Debug.Log($"Not enough money! Need ${unitType.cost}, have ${playerMoney}");
            FlashButton(button, Color.red);
            return;
        }
        
        // Check cooldown
        if (Time.time - unitType.lastSpawnTime < unitType.cooldown)
        {
            float remainingCooldown = unitType.cooldown - (Time.time - unitType.lastSpawnTime);
            Debug.Log($"{unitType.unitName} on cooldown! {remainingCooldown:F1}s remaining");
            FlashButton(button, Color.blue);
            return;
        }
        
        // Spawn the unit!
        SpawnUnitAtBase(unitType);
        
        // Deduct money and update cooldown
        playerMoney -= unitType.cost;
        unitType.lastSpawnTime = Time.time;
        UpdateMoneyDisplay();
        
        FlashButton(button, Color.green);
        Debug.Log($"Spawned {unitType.unitName}! Money: ${playerMoney}");
    }
    
    void SpawnUnitAtBase(UnitType unitType)
    {
        Vector3 spawnPosition = playerSpawnPoint != null ? 
            playerSpawnPoint.position : 
            new Vector3(-8, 0, 0); // Default left side spawn
            
        GameObject newUnit = Instantiate(unitType.unitPrefab, spawnPosition, Quaternion.identity);
        
        // Make sure unit has proper setup
        UnitBehavior unitBehavior = newUnit.GetComponent<UnitBehavior>();
        if (unitBehavior != null)
        {
            unitBehavior.isPlayerUnit = true;
            // The unit will automatically find and target the enemy base
        }
    }
    
    void FlashButton(Button button, Color flashColor)
    {
        StartCoroutine(FlashButtonCoroutine(button, flashColor));
    }
    
    System.Collections.IEnumerator FlashButtonCoroutine(Button button, Color flashColor)
    {
        Color originalColor = button.targetGraphic.color;
        button.targetGraphic.color = flashColor;
        yield return new WaitForSeconds(0.2f);
        button.targetGraphic.color = originalColor;
    }
    
    void UpdateMoneyDisplay()
    {
        if (moneyDisplay != null)
        {
            moneyDisplay.text = $"💰 ${playerMoney}";
        }
    }
}