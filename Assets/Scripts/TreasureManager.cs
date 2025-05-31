using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureManager : MonoBehaviour
{
    public static TreasureManager Instance;
    
    [Header("Treasure Settings")]
    public int totalTreasures = 20;
    public GameObject treasurePrefab;
    public GameObject endPointPrefab;
    public Transform player;
    
    [Header("Level Layout Settings")]
    public float levelWidth = 10f;
    public float treasureSpacing = 20f;
    public float endPointDistance = 500f;
    
    [Header("Spawn Settings")]
    public float minDistanceFromCenter = 2f;
    public float maxDistanceFromCenter = 4.5f;
    
    [Header("Sparkle Effect")]
    public GameObject sparkleEffectPrefab;
    
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
        FindPlayer();
        
        if (!ValidateReferences()) return;
        
        SpawnTreasuresAlongPath();
        SpawnEndPoint();
        
        Debug.Log($"Spawned {totalTreasures} treasures and endpoint");
    }
    
    void FindPlayer()
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
    }
    
    bool ValidateReferences()
    {
        if (treasurePrefab == null)
        {
            Debug.LogError("Treasure prefab not assigned!");
            return false;
        }
        
        if (endPointPrefab == null)
        {
            Debug.LogError("EndPoint prefab not assigned!");
            return false;
        }
        
        return true;
    }
    
    void SpawnTreasuresAlongPath()
    {
        for (int i = 0; i < totalTreasures; i++)
        {
            Vector2 treasurePos = GenerateRandomTreasurePosition(i);
            GameObject treasure = CreateTreasure(treasurePos, i);
            SetupTreasureComponents(treasure);
        }
    }
    
    Vector2 GenerateRandomTreasurePosition(int treasureIndex)
    {
        // Calculate Y position (going upward along the path)
        float yPos = (treasureIndex + 1) * treasureSpacing;
        
        // Generate random X position (left or right of center)
        float xPos = Random.Range(-maxDistanceFromCenter, maxDistanceFromCenter);
        
        // Ensure minimum distance from center line
        if (Mathf.Abs(xPos) < minDistanceFromCenter)
        {
            xPos = xPos >= 0 ? minDistanceFromCenter : -minDistanceFromCenter;
        }
        
        return new Vector2(xPos, yPos);
    }
    
    GameObject CreateTreasure(Vector2 position, int index)
    {
        GameObject treasure = Instantiate(treasurePrefab, position, Quaternion.identity);
        treasure.name = "Treasure_" + index;
        Debug.Log($"Spawned treasure {index} at position {position}");
        return treasure;
    }
    
    void SetupTreasureComponents(GameObject treasure)
    {
        // Get or add TreasureSpot component
        TreasureSpot treasureSpot = treasure.GetComponent<TreasureSpot>();
        if (treasureSpot == null)
        {
            treasureSpot = treasure.AddComponent<TreasureSpot>();
        }
        
        // Set player reference
        treasureSpot.player = player;
        
        // Create sparkle effect
        CreateSparkleEffect(treasure, treasureSpot);
    }
    
    void CreateSparkleEffect(GameObject treasure, TreasureSpot treasureSpot)
    {
        GameObject sparkle;
        
        if (sparkleEffectPrefab != null)
        {
            // Use provided sparkle prefab
            sparkle = Instantiate(sparkleEffectPrefab, treasure.transform);
            sparkle.name = "SparkleEffect";
        }
        else
        {
            // Create simple sparkle effect
            sparkle = CreateSimpleSparkleEffect(treasure.transform);
        }
        
        treasureSpot.sparkleEffect = sparkle;
        sparkle.SetActive(false);
    }
    
    GameObject CreateSimpleSparkleEffect(Transform parent)
    {
        GameObject sparkle = new GameObject("SparkleEffect");
        sparkle.transform.SetParent(parent);
        sparkle.transform.localPosition = Vector3.zero;
        
        // Create sparkle sprite
        SpriteRenderer renderer = sparkle.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSparkleSprite();
        renderer.sortingOrder = 10;
        
        // Add pulsing animation
        SparkleAnimation animation = sparkle.AddComponent<SparkleAnimation>();
        
        return sparkle;
    }
    
    Sprite CreateSparkleSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 4;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius);
                    colors[y * size + x] = new Color(1f, 1f, 0f, alpha); // Yellow sparkle
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
    
    void SpawnEndPoint()
    {
        Vector2 endPos = new Vector2(0, endPointDistance);
        endPoint = Instantiate(endPointPrefab, endPos, Quaternion.identity);
        endPoint.name = "EndPoint";
        
        // Ensure EndPoint script is attached
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
        
        // Optional: Add sound effect, score update, or other feedback here
    }
    
    public int GetCollectedTreasures()
    {
        return collectedTreasures;
    }
    
    public int GetTotalTreasures()
    {
        return totalTreasures;
    }
    
    // Additional methods for UI Counter compatibility
    public int GetCollectedCount()
    {
        return collectedTreasures;
    }
    
    public int GetTotalCount()
    {
        return totalTreasures;
    }
    
    public float GetCompletionPercentage()
    {
        return totalTreasures > 0 ? (float)collectedTreasures / totalTreasures * 100f : 0f;
    }
}

// Simple sparkle animation component
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