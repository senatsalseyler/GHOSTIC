using UnityEngine;
using UnityEngine.UI;

public class TreasureSpot : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 3f;
    public Transform player;
    
    [Header("Visual Effects")]
    public GameObject sparkleEffect;
    
    [Header("Interaction Settings")]
    public float interactionTime = 2f;
    private float randomInteractionTime;
    
    // UI Elements - created dynamically
    private GameObject progressBarContainer;
    private Image progressBarFill;
    
    private bool playerNearby = false;
    private bool isInteracting = false;
    private bool isCollected = false;
    private float currentProgress = 0f;
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        // Hide sparkle effect initially
        if (sparkleEffect != null)
            sparkleEffect.SetActive(false);
            
        // Set random interaction time (1-4 seconds)
        randomInteractionTime = Random.Range(1f, 4f);
        
        // Create progress bar UI
        CreateProgressBar();
        
        Debug.Log($"TreasureSpot {gameObject.name} initialized with interaction time: {randomInteractionTime}");
    }
    
    void CreateProgressBar()
    {
        // Create canvas for this treasure spot
        GameObject canvasObj = new GameObject("TreasureCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, 1.5f, 0); // Position above treasure
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        // Set appropriate scale for world space
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(3, 0.5f);
        canvasRect.localScale = Vector3.one * 0.1f; // Much larger scale than before
        
        // Create background for progress bar
        GameObject bgObj = new GameObject("ProgressBG");
        bgObj.transform.SetParent(canvasObj.transform);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark background
        
        // Create fill for progress bar
        GameObject fillObj = new GameObject("ProgressFill");
        fillObj.transform.SetParent(bgObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(0, 1); // Start with 0 width
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        progressBarFill = fillObj.AddComponent<Image>();
        progressBarFill.color = Color.green;
        
        progressBarContainer = canvasObj;
        progressBarContainer.SetActive(false);
    }
    
    void Update()
    {
        if (isCollected) return;
        
        CheckPlayerDistance();
        HandleInteraction();
        
        // Make progress bar face camera
        if (progressBarContainer != null && progressBarContainer.activeInHierarchy)
        {
            progressBarContainer.transform.LookAt(Camera.main.transform);
            progressBarContainer.transform.Rotate(0, 180, 0); // Flip to face camera correctly
        }
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
        
        // Use both mouse and touch input
        bool inputDown = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool inputHeld = Input.GetMouseButton(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved));
        bool inputUp = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
        
        if (inputDown)
        {
            StartInteraction();
        }
        else if (inputHeld && isInteracting)
        {
            ContinueInteraction();
        }
        else if (inputUp && isInteracting)
        {
            StopInteraction();
        }
        else if (!inputHeld && isInteracting)
        {
            // Handle case where input is released without InputUp being detected
            StopInteraction();
        }
    }
    
    void StartInteraction()
    {
        isInteracting = true;
        if (progressBarContainer != null)
            progressBarContainer.SetActive(true);
        currentProgress = 0f;
        UpdateProgressBar();
        Debug.Log($"Started interacting with treasure: {gameObject.name}");
    }
    
    void ContinueInteraction()
    {
        currentProgress += Time.deltaTime / randomInteractionTime;
        currentProgress = Mathf.Clamp01(currentProgress);
        UpdateProgressBar();
        
        if (currentProgress >= 1f)
        {
            CompleteInteraction();
        }
    }
    
    void UpdateProgressBar()
    {
        if (progressBarFill != null)
        {
            // Update the fill amount by changing the anchor
            RectTransform fillRect = progressBarFill.GetComponent<RectTransform>();
            fillRect.anchorMax = new Vector2(currentProgress, 1);
        }
    }
    
    void StopInteraction()
    {
        if (isInteracting)
        {
            isInteracting = false;
            if (progressBarContainer != null)
                progressBarContainer.SetActive(false);
            currentProgress = 0f;
            Debug.Log($"Stopped interacting with treasure: {gameObject.name}");
        }
    }
    
    void CompleteInteraction()
    {
        isCollected = true;
        isInteracting = false;
        
        if (sparkleEffect != null)
            sparkleEffect.SetActive(false);
        if (progressBarContainer != null)
            progressBarContainer.SetActive(false);
        
        Debug.Log($"Completed interaction with treasure: {gameObject.name}");
        
        // Notify the treasure manager
        if (TreasureManager.Instance != null)
        {
            TreasureManager.Instance.CollectTreasure();
        }
        
        // Hide this treasure spot
        gameObject.SetActive(false);
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize detection radius in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}