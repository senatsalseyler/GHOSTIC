using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureManager : MonoBehaviour
{
    public static TreasureManager Instance;
    
    [Header("Treasure Settings")]
    public int totalTreasures = 20;
    public GameObject treasurePrefab;
    public GameObject endPointPrefab; // The goal object
    public Transform player;
    
    [Header("Endless Level Settings")]
    public float levelWidth = 10f; // Width of the playable area
    public float treasureSpacing = 20f; // Vertical distance between treasure areas
    public float endPointDistance = 500f; // How far up the endpoint is
    
    [Header("Spawn Settings")]
    public float minDistanceFromCenter = 2f; // Min distance from center line
    public float maxDistanceFromCenter = 4.5f; // Max distance from center line
    
    [Header("Sparkle Effect")]
    public GameObject sparkleEffectPrefab; // Assign a particle system or visual effect
    
    private int collectedTreasures = 0;
    private GameObject endPoint;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("TreasureManager instance created");
        }
        else
        {
            Debug.Log("Duplicate TreasureManager destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Player found and assigned");
            }
            else
            {
                Debug.LogError("Player not found! Make sure player has 'Player' tag");
            }
        }
        
        if (treasurePrefab == null)
        {
            Debug.LogError("Treasure prefab not assigned!");
            return;
        }
        
        if (endPointPrefab == null)
        {
            Debug.LogError("EndPoint prefab not assigned!");
            return;
        }
        
        SpawnTreasuresAlongPath();
        SpawnEndPoint();
        
        Debug.Log($"Spawned {totalTreasures} treasures and endpoint");
    }
    
    void SpawnTreasuresAlongPath()
    {
        for (int i = 0; i < totalTreasures; i++)
        {
            // Calculate Y position (going upward)
            float yPos = (i + 1) * treasureSpacing;
            
            // Random X position (left or right of center)
            float xPos = Random.Range(-maxDistanceFromCenter, maxDistanceFromCenter);
            
            // Make sure it's not too close to center
            if (Mathf.Abs(xPos) < minDistanceFromCenter)
            {
                xPos = xPos >= 0 ? minDistanceFromCenter : -minDistanceFromCenter;
            }
            
            Vector2 treasurePos = new Vector2(xPos, yPos);
            
            GameObject treasure = Instantiate(treasurePrefab, treasurePos, Quaternion.identity);
            treasure.name = "Treasure_" + i;
            
            // Set up the treasure spot component
            TreasureSpot treasureSpot = treasure.GetComponent<TreasureSpot>();
            if (treasureSpot == null)
            {
                treasureSpot = treasure.AddComponent<TreasureSpot>();
            }
            
            // Set player reference
            treasureSpot.player = player;
            
            // Create sparkle effect if we have a prefab
            if (sparkleEffectPrefab != null)
            {
                GameObject sparkle = Instantiate(sparkleEffectPrefab, treasure.transform);
                sparkle.name = "SparkleEffect";
                treasureSpot.sparkleEffect = sparkle;
                sparkle.SetActive(false);
            }
            else
            {
                // Create a simple sparkle effect using a basic GameObject with renderer
                GameObject sparkle = CreateSimpleSparkleEffect(treasure.transform);
                treasureSpot.sparkleEffect = sparkle;
            }
            
            Debug.Log($"Spawned treasure {i} at position {treasurePos}");
        }
    }
    
    GameObject CreateSimpleSparkleEffect(Transform parent)
    {
        GameObject sparkle = new GameObject("SparkleEffect");
        sparkle.transform.SetParent(parent);
        sparkle.transform.localPosition = Vector3.zero;
        
        // Add a simple visual indicator (you can replace this with particle system)
        SpriteRenderer renderer = sparkle.AddComponent<SpriteRenderer>();
        
        // Create a simple circle sprite
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        Vector2 center = new Vector2(16, 16);
        
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= 8)
                {
                    float alpha = 1f - (distance / 8f);
                    colors[y * 32 + x] = new Color(1f, 1f, 0f, alpha); // Yellow with fade
                }
                else
                {
                    colors[y * 32 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        renderer.sprite = sprite;
        renderer.sortingOrder = 10;
        
        // Add pulsing animation
        SparkleAnimation animation = sparkle.AddComponent<SparkleAnimation>();
        
        sparkle.SetActive(false);
        return sparkle;
    }
    
    void SpawnEndPoint()
    {
        Vector2 endPos = new Vector2(0, endPointDistance);
        endPoint = Instantiate(endPointPrefab, endPos, Quaternion.identity);
        endPoint.name = "EndPoint";
        
        // Add EndPoint script if it doesn't exist
        EndPoint endPointScript = endPoint.GetComponent<EndPoint>();
        if (endPointScript == null)
        {
            endPointScript = endPoint.AddComponent<EndPoint>();
        }
        
        Debug.Log($"Spawned endpoint at position {endPos}");
    }
    
    public void CollectTreasure()
    {
        collectedTreasures++;
        Debug.Log($"Treasures collected: {collectedTreasures}/{totalTreasures}");
        
        // Optional: Add sound effect or other feedback here
    }
    
    public int GetCollectedTreasures()
    {
        return collectedTreasures;
    }
    
    public int GetTotalTreasures()
    {
        return totalTreasures;
    }
}

// Simple animation component for sparkle effect
public class SparkleAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float pulseSpeed = 2f;
    private float minAlpha = 0.3f;
    private float maxAlpha = 1f;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (spriteRenderer != null && gameObject.activeInHierarchy)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}