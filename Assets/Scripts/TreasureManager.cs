using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureManager : MonoBehaviour
{
    public static TreasureManager Instance;
    
    [Header("Treasure Settings")]
    public int totalTreasures = 20;
    public GameObject treasurePrefab;
    public Transform player;
    
    [Header("Memory Object Settings")]
    public GameObject memoryObjectPrefab;
    public string memoryInfoText = "Collect your first memory!";
    
    [Header("Level Layout Settings")]
    public float levelWidth = 10f;
    public float treasureSpacing = 20f;
    
    [Header("Spawn Settings")]
    public float minDistanceFromCenter = 2f;
    public float maxDistanceFromCenter = 4.5f;
    
    [Header("Sparkle Effect")]
    public GameObject sparkleEffectPrefab;
    
    private int collectedTreasures = 0;
    private GameObject memoryObject;
    private bool allTreasuresCollected = false;
    
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
        
        Debug.Log($"Spawned {totalTreasures} treasures");
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
    
    void SpawnMemoryObject()
    {
        if (memoryObject != null) return; // Already spawned
        
        // Calculate position - spawn at the end of the treasure path
        float lastTreasureY = totalTreasures * treasureSpacing;
        Vector2 memoryPos = new Vector2(0, lastTreasureY + treasureSpacing);
        
        if (memoryObjectPrefab != null)
        {
            memoryObject = Instantiate(memoryObjectPrefab, memoryPos, Quaternion.identity);
        }
        else
        {
            // Create default memory object if prefab not assigned
            memoryObject = CreateDefaultMemoryObject(memoryPos);
        }
        
        memoryObject.name = "FirstMemory";
        
        // Setup memory object component
        MemoryObject memoryComponent = memoryObject.GetComponent<MemoryObject>();
        if (memoryComponent == null)
        {
            memoryComponent = memoryObject.AddComponent<MemoryObject>();
        }
        
        memoryComponent.infoText = memoryInfoText;
        memoryComponent.player = player;
        
        Debug.Log($"Memory object spawned at position {memoryPos}");
    }
    
    GameObject CreateDefaultMemoryObject(Vector2 position)
    {
        GameObject memory = new GameObject("FirstMemory");
        memory.transform.position = position;
        
        // Create visual representation
        SpriteRenderer renderer = memory.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateMemorySprite();
        renderer.sortingOrder = 5;
        
        // Add collider for interaction
        CircleCollider2D collider = memory.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 1f;
        
        return memory;
    }
    
    Sprite CreateMemorySprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        Vector2 center = new Vector2(size / 2, size / 2);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= 25)
                {
                    if (distance <= 20)
                    {
                        // Create a gradient from purple to pink for memory object
                        float factor = distance / 20f;
                        Color memoryColor = Color.Lerp(new Color(0.8f, 0.2f, 0.8f, 1f), new Color(1f, 0.4f, 0.8f, 1f), factor);
                        colors[y * size + x] = memoryColor;
                    }
                    else
                    {
                        float alpha = 1f - ((distance - 20f) / 5f);
                        colors[y * size + x] = new Color(1f, 0.4f, 0.8f, alpha);
                    }
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
    
    public void CollectTreasure()
    {
        collectedTreasures++;
        Debug.Log($"Treasures collected: {collectedTreasures}/{totalTreasures}");
        
        // Check if all treasures are collected
        if (collectedTreasures >= totalTreasures && !allTreasuresCollected)
        {
            allTreasuresCollected = true;
            Debug.Log("All treasures collected! Spawning memory object...");
            SpawnMemoryObject();
        }
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
    
    public bool AreAllTreasuresCollected()
    {
        return allTreasuresCollected;
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