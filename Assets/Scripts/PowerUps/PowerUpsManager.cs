using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PowerUpsManager : MonoBehaviour
{

    public static PowerUpsManager Instance;

    [SerializeField] private GameObject throwingPowerUpPrefab;
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
        Assert.IsNotNull(throwingPowerUpPrefab);
        Assert.IsTrue(throwingPowerUpPrefab.GetComponent<PowerUp>());
    }

    /// <summary>
    /// Put power ups to bricks placed on the scene.
    /// </summary>
    public void LinkPowerUpsToBricks()
    {
        List<Brick> bricksWithNoAttachedPowerUp = new List<Brick>(BricksManager.Instance.AllBricks);
        
        // Put randomly power ups to bricks
        foreach (SOPowerUp powerUp in powerUps)
        {
            if (bricksWithNoAttachedPowerUp.Count == 0)
            {
                break;
            }
            Brick randomBrick = bricksWithNoAttachedPowerUp[Random.Range(0, bricksWithNoAttachedPowerUp.Count)];
            
            randomBrick.SetUpPowerUp(powerUp);
            
            bricksWithNoAttachedPowerUp.Remove(randomBrick);
        }
    }

    /// <summary>
    /// Spawn new throwing power up in given coordinates.
    /// </summary>
    /// <param name="powerUp"></param>
    /// <param name="position"></param>
    public void SpawnThrowingPowerUp(SOPowerUp powerUp, Vector2 position)
    {
        GameObject throwingPowerUpInstance = throwingPowerUpPrefab;
        throwingPowerUpInstance.GetComponent<PowerUp>().Type = powerUp.Type;
        throwingPowerUpInstance.GetComponent<SpriteRenderer>().sprite = powerUp.Sprite;
        Instantiate(throwingPowerUpInstance, position, Quaternion.identity);
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
                BallsManager.Instance.EnablePickedPowerUp(powerUpType);
                break;
            
            case EPowerUp.PlatformBigger:
            case EPowerUp.PlatformSmaller:
                PlatformsManager.Instance.ActivatePlatformPowerUp(powerUpType);
                break;
        }
    }
}
