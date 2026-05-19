using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PowerUpsManager : MonoBehaviour
{

    public static PowerUpsManager Instance;

    [SerializeField] private List<SOPowerUp> powerUps;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>PowerUpsManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Put power ups to bricks placed on the scene.
    /// </summary>
    public void LinkPowerUpsToBricks()
    {
        List<Brick> bricks = BricksManager.Instance.AllBricks.ToList();
        
        // Put randomly power ups to bricks
        foreach (SOPowerUp powerUpSo in powerUps)
        {
            PowerUp powerUp = powerUpSo.PowerUpPrefab.gameObject.GetComponent<PowerUp>();
            Assert.IsNotNull(powerUp);
            
            Brick randomBrick = bricks[Random.Range(0, bricks.Count)];
            
            Debug.Log($"[<color=orange>PowerUpsManager / {name}</color>] Put power up {powerUp.PowerUpType} to brick {randomBrick.name}.");
            randomBrick.SetUpPowerUp(powerUpSo);
            
            bricks.Remove(randomBrick);
        }
        
        Debug.Log($"[<color=orange>PowerUpsManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnPowerUpsManagerSuccesfullyNotified();
    }

    /// <summary>
    /// Enable given power up.
    /// </summary>
    /// <param name="powerUpType"></param>
    public void ActivatePowerUp(EPowerUp powerUpType)
    {
        Debug.Log($"[<color=orange>PowerUpsManager / {name}</color>] Received notification : Activate power up {powerUpType}.");

        switch (powerUpType)
        {
            case EPowerUp.BallBigger:
            case EPowerUp.BallSmaller:
                BallsManager.Instance.ActivateBallPowerUp(powerUpType);
                break;
            
            case EPowerUp.PlatformBigger:
            case EPowerUp.PlatformSmaller:
                PlatformsManager.Instance.ActivatePlatformPowerUp(powerUpType);
                break;
        }
    }
}
