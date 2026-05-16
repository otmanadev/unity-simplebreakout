using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BallsManager : MonoBehaviour
{

    public static BallsManager Instance;
    private List<Ball> _balls;
    
    [Header("Ball Datas")]
    [SerializeField] private List<SOBallSize> ballSizes;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[BallsManager / {name}] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(ballSizes);
        _balls = new List<Ball>();
    }

    /// <summary>
    /// Receive notification from ball when he is created.
    /// </summary>
    /// <param name="ball"></param>
    public void OnBallCreatedNotification(Ball ball)
    {
        if (_balls.Contains(ball))
        {
            return;
        }
        
        _balls.Add(ball);
        Debug.Log($"[BallsManager / {name}] Received notification from {ball.name} : Ball created.");
    }
    
    /// <summary>
    /// Receive notification from ball when he is created.
    /// </summary>
    /// <param name="ball"></param>
    public void OnBallReachDeadZoneNotification(Ball ball)
    {
        if (!_balls.Contains(ball))
        {
            return;
        }
        
        Debug.Log($"[BallsManager / {name}] Received notification from {ball.name} : Ball reached dead zone.");
        Debug.Log($"[BallsManager / {name}] Send notification from {GameManager.Instance.name} : Ball reached dead zone.");
        GameManager.Instance.OnBallReachedDeadZoneNotification();
    }

    /// <summary>
    /// Enable given power up for every balls.
    /// </summary>
    /// <param name="powerUpType"></param>
    public void ActivateBallPowerUp(EPowerUp powerUpType)
    {
        foreach (Ball ball in _balls)
        {
            switch (powerUpType)
            {
                case EPowerUp.BallBigger:
                case EPowerUp.BallSmaller:
                    SOBallSize currentSoBallSize = ball.SoBallSize;
                    SOBallSize newSoBallSize = PickUpNewBallSizeSoForCurrentBall(powerUpType, currentSoBallSize);
                    ball.SetUpNewBallSize(newSoBallSize);
                    break;
            }
        }
    }

    /// <summary>
    /// Select ball size SO from current ball size and depending on selected power up.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="currentSoBallSize"></param>
    /// <returns></returns>
    private SOBallSize PickUpNewBallSizeSoForCurrentBall(EPowerUp powerUpType, SOBallSize currentSoBallSize)
    {
        if (!powerUpType.Equals(EPowerUp.BallSmaller) && !powerUpType.Equals(EPowerUp.BallBigger))
        {
            Debug.LogError($"[BallsManager / {name}] Cannot find BallSizeSO from power up type {powerUpType}");
            return null;
        }
        
        EBallSize currentBallSizeType = currentSoBallSize.BallSize;
        EBallSize newBallSizeType = currentBallSizeType;
        switch (currentBallSizeType)
        {
            case EBallSize.ExtraLarge:
                newBallSizeType = powerUpType.Equals(EPowerUp.BallSmaller) 
                    ? EBallSize.Large 
                    : newBallSizeType;
                break;
            case EBallSize.Large:
                newBallSizeType = powerUpType.Equals(EPowerUp.BallBigger) 
                    ? EBallSize.ExtraLarge 
                    : EBallSize.Medium;
                break;
            case EBallSize.Medium:
                newBallSizeType = powerUpType.Equals(EPowerUp.BallBigger) 
                    ? EBallSize.Large 
                    : EBallSize.Small;
                break;
            case EBallSize.Small:
                newBallSizeType = powerUpType.Equals(EPowerUp.BallBigger) 
                    ? EBallSize.Medium 
                    : EBallSize.ExtraSmall;
                break;
            case EBallSize.ExtraSmall:
                newBallSizeType = powerUpType.Equals(EPowerUp.BallBigger) 
                    ? EBallSize.Small 
                    : newBallSizeType;
                break;
        }

        SOBallSize newSoBallSize = ballSizes.Find(e => e.BallSize.Equals(newBallSizeType));
        if (newSoBallSize == null)
        {
            Debug.LogError($"[BallsManager / {name}] Cannot find BallSizeSO from size type {newBallSizeType}");
            return null;
        }

        return newSoBallSize;
    }
    
}
