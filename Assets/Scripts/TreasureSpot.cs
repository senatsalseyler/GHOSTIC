using UnityEngine;

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
    
    // Simple progress bar using a sprite renderer
    private GameObject progressBarBG;
    private GameObject progressBarFill;
    private SpriteRenderer progressFillRenderer;
    
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
        
        // Create simple progress bar
        CreateSimpleProgressBar();
        
        Debug.Log($"TreasureSpot {gameObject.name} initialized with interaction time: {randomInteractionTime}");
    }
    
    void CreateSimpleProgressBar()
    {
        // Create background bar
        progressBarBG = new GameObject("ProgressBG");
        progressBarBG.transform.SetParent(transform);
        progressBarBG.transform.localPosition = new Vector3(0, 1.2f, 0);
        
        SpriteRenderer bgRenderer = progressBarBG.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = CreateBarSprite(Color.black, 100, 10);
        bgRenderer.sortingOrder = 9;
        
        // Create fill bar
        progressBarFill = new GameObject("ProgressFill");
        progressBarFill.transform.SetParent(progressBarBG.transform);
        progressBarFill.transform.localPosition = new Vector3(-0.5f, 0, -0.1f); // Offset to left edge
        
        progressFillRenderer = progressBarFill.AddComponent<SpriteRenderer>();
        progressFillRenderer.sprite = CreateBarSprite(Color.white, 100, 8);
        progressFillRenderer.sortingOrder = 10;
        
        // Start with no fill
        progressBarFill.transform.localScale = new Vector3(0, 1, 1);
        
        // Hide initially
        progressBarBG.SetActive(false);
    }
    
    Sprite CreateBarSprite(Color color, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
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
        else if (!Input.GetMouseButton(0) && isInteracting)
        {
            StopInteraction();
        }
    }
    
    void StartInteraction()
    {
        isInteracting = true;
        if (progressBarBG != null)
            progressBarBG.SetActive(true);
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
            // Scale the fill bar based on progress
            progressBarFill.transform.localScale = new Vector3(currentProgress, 1, 1);
        }
    }
    
    void StopInteraction()
    {
        if (isInteracting)
        {
            isInteracting = false;
            if (progressBarBG != null)
                progressBarBG.SetActive(false);
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
        if (progressBarBG != null)
            progressBarBG.SetActive(false);
        
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