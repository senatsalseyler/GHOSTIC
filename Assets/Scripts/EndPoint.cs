using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    [Header("End Point Settings")]
    public float detectionRadius = 2f;
    
    [Header("Visual Feedback")]
    public GameObject sparkleEffect;
    public Color endPointColor = Color.cyan;
    
    private Transform player;
    private bool levelCompleted = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("EndPoint: Player found and assigned");
        }
        else
        {
            Debug.LogError("EndPoint: Player not found! Make sure player has 'Player' tag");
        }
        
        // Set up visual appearance
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            CreateEndPointSprite();
        }
        
        // Create sparkle effect if not assigned
        if (sparkleEffect == null)
        {
            CreateSparkleEffect();
        }
        
        Debug.Log("EndPoint initialized at position: " + transform.position);
    }
    
    void CreateEndPointSprite()
    {
        // Create a simple endpoint sprite
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        Vector2 center = new Vector2(32, 32);
        
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= 20)
                {
                    if (distance <= 15)
                    {
                        colors[y * 64 + x] = endPointColor;
                    }
                    else
                    {
                        float alpha = 1f - ((distance - 15f) / 5f);
                        colors[y * 64 + x] = new Color(endPointColor.r, endPointColor.g, endPointColor.b, alpha);
                    }
                }
                else
                {
                    colors[y * 64 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 5;
    }
    
    void CreateSparkleEffect()
    {
        GameObject sparkle = new GameObject("EndPointSparkle");
        sparkle.transform.SetParent(transform);
        sparkle.transform.localPosition = Vector3.zero;
        
        SpriteRenderer sparkleRenderer = sparkle.AddComponent<SpriteRenderer>();
        
        // Create sparkle texture
        Texture2D texture = new Texture2D(48, 48);
        Color[] colors = new Color[48 * 48];
        Vector2 center = new Vector2(24, 24);
        
        for (int x = 0; x < 48; x++)
        {
            for (int y = 0; y < 48; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= 12)
                {
                    float alpha = 1f - (distance / 12f);
                    colors[y * 48 + x] = new Color(1f, 1f, 1f, alpha * 0.8f); // White sparkle
                }
                else
                {
                    colors[y * 48 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 48, 48), new Vector2(0.5f, 0.5f));
        sparkleRenderer.sprite = sprite;
        sparkleRenderer.sortingOrder = 15;
        
        // Add animation
        EndPointAnimation animation = sparkle.AddComponent<EndPointAnimation>();
        
        sparkleEffect = sparkle;
    }
    
    void Update()
    {
        if (levelCompleted || player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= detectionRadius)
        {
            CompleteLevel();
        }
    }
    
    void CompleteLevel()
    {
        levelCompleted = true;
        
        int treasuresCollected = 0;
        int totalTreasures = 0;
        
        if (TreasureManager.Instance != null)
        {
            treasuresCollected = TreasureManager.Instance.GetCollectedTreasures();
            totalTreasures = TreasureManager.Instance.GetTotalTreasures();
        }
        
        Debug.Log($"Level Complete! Collected {treasuresCollected}/{totalTreasures} treasures!");
        
        // Calculate completion percentage
        float completionPercentage = totalTreasures > 0 ? (float)treasuresCollected / totalTreasures * 100f : 0f;
        Debug.Log($"Completion: {completionPercentage:F1}%");
        
        // You can add more completion logic here
        // For example: unlock next level, show completion UI, etc.
        
        // For now, just reload the scene after a delay
        //Invoke(nameof(ReloadScene), 3f);
    }
    
    /*void ReloadScene()
    {
        Debug.Log("Reloading scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }*/
    
    void OnDrawGizmosSelected()
    {
        // Visualize detection radius in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

// Animation component for endpoint sparkle effect
public class EndPointAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float rotationSpeed = 30f;
    private float pulseSpeed = 1.5f;
    private float minScale = 0.8f;
    private float maxScale = 1.2f;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        // Rotate the sparkle
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Pulse the scale
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        transform.localScale = Vector3.one * scale;
        
        // Pulse the alpha
        if (spriteRenderer != null)
        {
            float alpha = Mathf.Lerp(0.4f, 0.9f, (Mathf.Sin(Time.time * pulseSpeed * 1.5f) + 1f) * 0.5f);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}