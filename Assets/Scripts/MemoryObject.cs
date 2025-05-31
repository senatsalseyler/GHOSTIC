using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemoryObject : MonoBehaviour
{
    [Header("Memory Object Settings")]
    public float detectionRadius = 2f;
    public string infoText = "Collect your first memory!";
    public string cutsceneSceneName = "CutsceneScene"; // Scene name for cutscenes
    
    [Header("UI Settings")]
    public GameObject infoUI;
    public Text infoTextComponent;
    public Button collectButton;
    
    [Header("Visual Effects")]
    public GameObject glowEffect;
    
    public Transform player;
    private bool playerInRange = false;
    private bool memoryCollected = false;
    private Canvas worldCanvas;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("MemoryObject: Player found and assigned");
            }
            else
            {
                Debug.LogError("MemoryObject: Player not found! Make sure player has 'Player' tag");
            }
        }
        
        // Create UI if not assigned
        if (infoUI == null)
        {
            CreateInfoUI();
        }
        
        // Create glow effect if not assigned
        if (glowEffect == null)
        {
            CreateGlowEffect();
        }
        
        // Hide UI initially
        if (infoUI != null)
        {
            infoUI.SetActive(false);
        }
        
        Debug.Log("MemoryObject initialized at position: " + transform.position);
    }
    
    void CreateInfoUI()
    {
        // Create world space canvas
        GameObject canvasObj = new GameObject("MemoryInfoCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, 2f, 0); // Position above the memory object
        
        worldCanvas = canvasObj.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.worldCamera = Camera.main;
        worldCanvas.sortingOrder = 100;
        
        // Set canvas size
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(300, 100);
        canvasRect.localScale = Vector3.one * 0.01f; // Scale down for world space
        
        // Create background panel
        GameObject panelObj = new GameObject("InfoPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create info text
        GameObject textObj = new GameObject("InfoText");
        textObj.transform.SetParent(panelObj.transform, false);
        
        infoTextComponent = textObj.AddComponent<Text>();
        infoTextComponent.text = infoText;
        infoTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        infoTextComponent.fontSize = 16;
        infoTextComponent.color = Color.white;
        infoTextComponent.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.5f);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, -10);
        
        // Create collect button
        GameObject buttonObj = new GameObject("CollectButton");
        buttonObj.transform.SetParent(panelObj.transform, false);
        
        collectButton = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green button
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.2f, 0f);
        buttonRect.anchorMax = new Vector2(0.8f, 0.4f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        // Button text
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.text = "Collect";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        // Add button click event
        collectButton.onClick.AddListener(OnCollectButtonClicked);
        
        infoUI = canvasObj;
    }
    
    void CreateGlowEffect()
    {
        GameObject glow = new GameObject("MemoryGlow");
        glow.transform.SetParent(transform);
        glow.transform.localPosition = Vector3.zero;
        
        SpriteRenderer glowRenderer = glow.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = CreateGlowSprite();
        glowRenderer.sortingOrder = -1; // Behind the main object
        
        // Add glow animation
        MemoryGlowAnimation glowAnimation = glow.AddComponent<MemoryGlowAnimation>();
        
        glowEffect = glow;
    }
    
    Sprite CreateGlowSprite()
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        Vector2 center = new Vector2(size / 2, size / 2);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= 50)
                {
                    float alpha = 1f - (distance / 50f);
                    alpha = Mathf.Pow(alpha, 2); // Make the glow more concentrated in the center
                    colors[y * size + x] = new Color(1f, 0.8f, 0.2f, alpha * 0.6f); // Golden glow
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    void Update()
    {
        if (memoryCollected || player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= detectionRadius && !playerInRange)
        {
            playerInRange = true;
            ShowInfoUI();
        }
        else if (distance > detectionRadius && playerInRange)
        {
            playerInRange = false;
            HideInfoUI();
        }
    }
    
    void ShowInfoUI()
    {
        if (infoUI != null)
        {
            infoUI.SetActive(true);
            Debug.Log("Memory info UI shown");
        }
    }
    
    void HideInfoUI()
    {
        if (infoUI != null)
        {
            infoUI.SetActive(false);
            Debug.Log("Memory info UI hidden");
        }
    }
    
    void OnCollectButtonClicked()
    {
        if (memoryCollected) return;
        
        memoryCollected = true;
        Debug.Log("First memory collected! Loading cutscene...");
        
        HideInfoUI();
        
        // Store current scene name to return to it later
        PlayerPrefs.SetString("ReturnScene", SceneManager.GetActiveScene().name);
        
        // Load cutscene scene (placeholder for now)
        LoadCutsceneScene();
    }
    
    void LoadCutsceneScene()
    {
        // For now, we'll just show a debug message and simulate returning
        // You can replace this with actual scene loading when cutscenes are ready
        Debug.Log($"Would load cutscene scene: {cutsceneSceneName}");
        
        // Simulate cutscene completion - in real implementation, this would be called
        // from the cutscene scene when it's finished
        Invoke(nameof(SimulateCutsceneComplete), 2f);
    }
    
    void SimulateCutsceneComplete()
    {
        Debug.Log("Cutscene completed (simulated)");
        // In real implementation, the cutscene scene would handle returning to the game
        // For now, we'll just destroy the memory object to show it was collected
        DestroyMemoryObject();
    }
    
    void DestroyMemoryObject()
    {
        Debug.Log("Memory object collected and removed");
        Destroy(gameObject);
    }
    
    // This method can be called from a cutscene manager when returning from cutscenes
    public static void ReturnFromCutscene()
    {
        string returnScene = PlayerPrefs.GetString("ReturnScene", "");
        if (!string.IsNullOrEmpty(returnScene))
        {
            Debug.Log($"Returning to scene: {returnScene}");
            SceneManager.LoadScene(returnScene);
        }
        else
        {
            Debug.LogWarning("No return scene found!");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize detection radius in editor
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

// Animation component for memory object glow effect
public class MemoryGlowAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float pulseSpeed = 1f;
    private float rotationSpeed = 20f;
    private float minAlpha = 0.3f;
    private float maxAlpha = 0.8f;
    private Vector3 baseScale;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
    }
    
    void Update()
    {
        // Rotate the glow
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Pulse the alpha and scale
        float pulseValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        
        if (spriteRenderer != null)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulseValue);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        
        // Pulse the scale slightly
        float scaleMultiplier = Mathf.Lerp(0.9f, 1.1f, pulseValue);
        transform.localScale = baseScale * scaleMultiplier;
    }
}