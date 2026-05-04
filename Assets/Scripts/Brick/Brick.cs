using NUnit.Framework;
using UnityEngine;

public class Brick : MonoBehaviour
{
    
    private readonly string _materialPropertyMainTex = "_MainTex";
    private readonly string _materialPropertyColor = "_Color";
    private readonly string _materialGlowEnabled = "_GlowEnabled";
    private readonly string _materialGlowColor = "_GlowColor";
    private readonly string _materialGlowStrength = "_GlowStrength";

    private BoxCollider2D _boxCollider2D;
    
    [Header("Sprites and materials properties")]
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private SpriteRenderer fillRenderer;
    
    [Header("Power Up properties")]
    [SerializeField] private GameObject powerUpGO;
    
    protected virtual void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(_boxCollider2D);
        
        Assert.IsNotNull(outlineRenderer);
        Assert.IsNotNull(fillRenderer);
        PickRandomGradientColor();
    }

    protected virtual void Start()
    {
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick created.");
        BricksManager.Instance.OnBrickCreatedNotification(this);
    }

    /// <summary>
    /// Select random color for material gradient color.
    /// </summary>
    private void PickRandomGradientColor()
    {
        fillRenderer.material.SetTexture(_materialPropertyMainTex, fillRenderer.sprite.texture);
        fillRenderer.material.SetColor(_materialPropertyColor, BrickColor.PickRandomColor());
    }
    
    /// <summary>
    /// Receive given power up.
    /// </summary>
    /// <param name="powerUpSO"></param>
    public void SetUpPowerUp(PowerUpSO powerUpSO)
    {
        PowerUp powerUp = powerUpSO.PowerUpPrefab.gameObject.GetComponent<PowerUp>();
        Debug.Log($"[Brick / {name}] Received power up {powerUp.PowerUpType}.");
        powerUpGO = powerUpSO.PowerUpPrefab;
        
        fillRenderer.material.SetColor(_materialGlowColor, powerUpSO.GlowColor);
        fillRenderer.material.SetFloat(_materialGlowStrength, powerUpSO.GlowStrength);
        fillRenderer.material.SetInt(_materialGlowEnabled, 1);
    }

    /// <summary>
    /// Hit brick, so he could die... Or not...
    /// </summary>
    public void TryHitBrick()
    {
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick destroyed.");
        BricksManager.Instance.OnBrickDestroyedNotification(this);
        SpawnPowerUp();
        Destroy(gameObject);
    }

    /// <summary>
    /// Appears power up if attached.
    /// </summary>
    private void SpawnPowerUp()
    {
        if (powerUpGO == null)
            return;
        
        Instantiate(powerUpGO, transform.position, Quaternion.identity);
    }
    
}
