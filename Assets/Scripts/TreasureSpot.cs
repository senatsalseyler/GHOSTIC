using UnityEngine;
using UnityEngine.UI;

public class TreasureSpot : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 3f;
    public Transform player;
    
    [Header("Visual Effects")]
    public GameObject sparkleEffect;
    
    [Header("UI Elements")]
    public Canvas worldCanvas; // World space canvas for this treasure
    public GameObject progressBarUI;
    public Slider progressBar;
    public Image treasureIcon;
    public Sprite treasureSprite;
    
    [Header("Interaction Settings")]
    public float interactionTime = 2f;
    private float randomInteractionTime;
    
    private bool playerNearby = false;
    private bool isInteracting = false;
    private bool isCollected = false;
    private float currentProgress = 0f;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
        // Create world canvas if it doesn't exist
        if (worldCanvas == null)
        {
            CreateWorldCanvas();
        }
        
        // Hide UI elements initially
        if (sparkleEffect != null)
            sparkleEffect.SetActive(false);
        if (progressBarUI != null)
            progressBarUI.SetActive(false);
        
        // Set treasure sprite
        if (treasureIcon != null && treasureSprite != null)
            treasureIcon.sprite = treasureSprite;
            
        // Set random interaction time (1-4 seconds)
        randomInteractionTime = Random.Range(1f, 4f);
        
        Debug.Log($"TreasureSpot {gameObject.name} initialized with interaction time: {randomInteractionTime}");
    }
    
    void CreateWorldCanvas()
    {
        // Create a world space canvas for this treasure
        GameObject canvasObj = new GameObject("TreasureCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;
        
        worldCanvas = canvasObj.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.worldCamera = mainCamera;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 0.01f;
        
        // Create progress bar UI if it doesn't exist
        if (progressBarUI == null)
        {
            CreateProgressBarUI();
        }
    }
    
    void CreateProgressBarUI()
    {
        // Create progress bar background
        GameObject bgObj = new GameObject("ProgressBarBG");
        bgObj.transform.SetParent(worldCanvas.transform);
        bgObj.transform.localPosition = new Vector3(0, 100, 0);
        bgObj.transform.localScale = Vector3.one;
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.5f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(200, 30);
        
        // Create slider
        GameObject sliderObj = new GameObject("ProgressSlider");
        sliderObj.transform.SetParent(bgObj.transform);
        sliderObj.transform.localPosition = Vector3.zero;
        sliderObj.transform.localScale = Vector3.one;
        
        progressBar = sliderObj.AddComponent<Slider>();
        progressBar.minValue = 0f;
        progressBar.maxValue = 1f;
        progressBar.value = 0f;
        
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;
        
        // Create fill area
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform);
        RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        // Create fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.green;
        
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        progressBar.fillRect = fillRect;
        
        progressBarUI = bgObj;
        progressBarUI.SetActive(false);
    }
    
    void Update()
    {
        if (isCollected) return;
        
        CheckPlayerDistance();
        HandleInteraction();
    }
    
    void CheckPlayerDistance()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasNearby = playerNearby;
        playerNearby = distance <= detectionRadius;
        
        if (playerNearby && !wasNearby)
        {
            // Player entered detection range
            if (sparkleEffect != null)
                sparkleEffect.SetActive(true);
            Debug.Log($"Player near treasure: {gameObject.name}");
        }
        else if (!playerNearby && wasNearby)
        {
            // Player left detection range
            if (sparkleEffect != null)
                sparkleEffect.SetActive(false);
            StopInteraction();
            Debug.Log($"Player left treasure: {gameObject.name}");
        }
    }
    
    void HandleInteraction()
    {
        if (!playerNearby) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            StartInteraction();
        }
        else if (Input.GetMouseButton(0) && isInteracting)
        {
            ContinueInteraction();
        }
        else if (Input.GetMouseButtonUp(0) && isInteracting)
        {
            StopInteraction();
        }
    }
    
    void StartInteraction()
    {
        isInteracting = true;
        if (progressBarUI != null)
            progressBarUI.SetActive(true);
        currentProgress = 0f;
        if (progressBar != null)
            progressBar.value = 0f;
        Debug.Log($"Started interacting with treasure: {gameObject.name}");
    }
    
    void ContinueInteraction()
    {
        currentProgress += Time.deltaTime / randomInteractionTime;
        if (progressBar != null)
            progressBar.value = currentProgress;
        
        if (currentProgress >= 1f)
        {
            CompleteInteraction();
        }
    }
    
    void StopInteraction()
    {
        isInteracting = false;
        if (progressBarUI != null)
            progressBarUI.SetActive(false);
        currentProgress = 0f;
        if (isInteracting)
            Debug.Log($"Stopped interacting with treasure: {gameObject.name}");
    }
    
    void CompleteInteraction()
    {
        isCollected = true;
        if (sparkleEffect != null)
            sparkleEffect.SetActive(false);
        if (progressBarUI != null)
            progressBarUI.SetActive(false);
        
        Debug.Log($"Completed interaction with treasure: {gameObject.name}");
        
        // Notify the game manager
        if (TreasureManager.Instance != null)
        {
            TreasureManager.Instance.CollectTreasure();
        }
        
        // Hide or destroy this treasure spot
        gameObject.SetActive(false);
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize detection radius in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}