using NUnit.Framework;
using UnityEngine;

public class Brick : MonoBehaviour
{

    private BoxCollider2D _boxCollider2D;
    [SerializeField] private GameObject powerUpGo;

    [Header("Health")]
    [SerializeField, Min(0)] private int health;
    
    protected virtual void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(_boxCollider2D);
    }

    protected virtual void Start()
    {
        Debug.Log($"[Brick / {name}] Send notification to Bricks Manager : <color=orange>Brick initialized</color>");
        BricksManager.Instance.OnBrickInitializedNotification(this);
    }
    
    /// <summary>
    /// Receive given power up.
    /// </summary>
    /// <param name="givenSoPowerUpGo"></param>
    public void SetUpPowerUp(SOPowerUp givenSoPowerUpGo)
    {
        PowerUp powerUp = givenSoPowerUpGo.PowerUpPrefab.gameObject.GetComponent<PowerUp>();
        Debug.Log($"[Brick / {name}] Received power up {powerUp.PowerUpType}.");
        powerUpGo = givenSoPowerUpGo.PowerUpPrefab;
    }

    /// <summary>
    /// Hit brick with ball damages.
    /// If brick has no health points left, he is destroyed.
    /// </summary>
    /// <param name="givenDamage"></param>
    public void TryHitBrick(int givenDamage)
    {
        health -= givenDamage;
        Debug.Log($"[Brick / {name}] Received {givenDamage} damage. Now has {health} health point(s) left.");

        if (health > 0)
            return;
        
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick destroyed.");
        BricksManager.Instance.OnBrickDestroyedNotification(this);
        SpawnAttachedPowerUp();
        Destroy(gameObject);
    }

    /// <summary>
    /// Appears power up if attached.
    /// </summary>
    private void SpawnAttachedPowerUp()
    {
        if (powerUpGo == null)
            return;
        
        Instantiate(powerUpGo, transform.position, Quaternion.identity);
    }
    
}
