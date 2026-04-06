using NUnit.Framework;
using UnityEngine;

public class Brick : MonoBehaviour
{
    
    private SpriteRenderer _spriteRenderer;
    
    private BoxCollider2D _boxCollider2D;
    
    [SerializeField] private GameObject powerUpGO;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(_spriteRenderer);
        
        _boxCollider2D = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(_boxCollider2D);
    }

    private void Start()
    {
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick created.");
        BricksManager.Instance.OnBrickCreatedNotification(this);
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
        _spriteRenderer.material = powerUpSO.Material;
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
