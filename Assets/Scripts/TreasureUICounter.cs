using UnityEngine;
using UnityEngine.UI;

public class TreasureUICounter : MonoBehaviour
{
    [Header("UI References")]
    public Text counterText;
    public Canvas canvas;
    
    [Header("UI Settings")]
    public float marginFromEdge = 20f;
    
    private GameObject counterObject;
    
    void Start()
    {
        CreateTreasureCounter();
    }
    
    void CreateTreasureCounter()
    {
        // Find canvas if not assigned
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // Create a canvas if none exists
                GameObject canvasObj = new GameObject("TreasureCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
        }
        
        // Create counter object
        counterObject = new GameObject("TreasureCounter");
        counterObject.transform.SetParent(canvas.transform, false);
        
        // Add text component
        counterText = counterObject.AddComponent<Text>();
        counterText.text = "0/0";
        counterText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        counterText.fontSize = 24;
        counterText.color = Color.white;
        counterText.fontStyle = FontStyle.Bold;
        
        // Add outline for better visibility
        Outline outline = counterObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, -1);
        
        // Position in upper left
        RectTransform rectTransform = counterObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(marginFromEdge, -marginFromEdge);
        rectTransform.sizeDelta = new Vector2(100, 30);
        
        // Update initial counter
        UpdateCounter();
    }
    
    void Update()
    {
        UpdateCounter();
    }
    
    void UpdateCounter()
    {
        if (counterText != null && TreasureManager.Instance != null)
        {
            int collected = TreasureManager.Instance.GetCollectedTreasures();
            int total = TreasureManager.Instance.GetTotalTreasures();
            counterText.text = $"{collected}/{total}";
        }
    }
}