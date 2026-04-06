using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PowerUpsManager : MonoBehaviour
{

    public static PowerUpsManager Instance;

    [SerializeField] private List<PowerUpSO> powerUps;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[PowerUpsManager / {name}] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetUpPowerUpsToBricks();
    }

    /// <summary>
    /// Put power ups to bricks placed on the scene.
    /// </summary>
    private void SetUpPowerUpsToBricks()
    {
        // Get Bricks
        List<Brick> bricks = GameObject.FindGameObjectsWithTag("Brick")
            .Select(o => o.GetComponent<Brick>())
            .Where(o => o != null)
            .ToList();
        
        // Put randomly power ups to bricks
        foreach (PowerUpSO powerUpSO in powerUps)
        {
            PowerUp powerUp = powerUpSO.PowerUpPrefab.gameObject.GetComponent<PowerUp>();
            Assert.IsNotNull(powerUp);
            
            Brick randomBrick = bricks[Random.Range(0, bricks.Count)];
            
            Debug.Log($"[PowerUpsManager / {name}] Put power up {powerUp.PowerUpType} to brick {randomBrick.name}.");
            randomBrick.SetUpPowerUp(powerUpSO);
            
            bricks.Remove(randomBrick);
        }
    }
}
