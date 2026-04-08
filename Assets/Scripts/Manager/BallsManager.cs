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
    [SerializeField] private List<BallSizeSO> ballSizes;
    
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
                case EPowerUp.BALL_BIGGER:
                case EPowerUp.BALL_SMALLER:
                    BallSizeSO currentBallSize = ball.BallSizeSo;
                    BallSizeSO newBallSize = PickUpNewBallSizeSoForCurrentBall(powerUpType, currentBallSize);
                    ball.SetUpNewBallSize(newBallSize);
                    break;
            }
        }
    }

    /// <summary>
    /// Select ball size SO from current ball size and depending on selected power up.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="currentBallSize"></param>
    /// <returns></returns>
    private BallSizeSO PickUpNewBallSizeSoForCurrentBall(EPowerUp powerUpType, BallSizeSO currentBallSize)
    {
        EBallSize currentBallSizeType = currentBallSize.BallSizeType;
        EBallSize newBallSizeType = currentBallSizeType;
        switch (currentBallSizeType)
        {
            case EBallSize.XLARGE:
                if (powerUpType.Equals(EPowerUp.BALL_SMALLER))
                    newBallSizeType = EBallSize.LARGE;
                break;
            case EBallSize.LARGE:
                if (powerUpType.Equals(EPowerUp.BALL_BIGGER))
                    newBallSizeType = EBallSize.XLARGE;
                if (powerUpType.Equals(EPowerUp.BALL_SMALLER))
                    newBallSizeType = EBallSize.MEDIUM;
                break;
            case EBallSize.MEDIUM:
                if (powerUpType.Equals(EPowerUp.BALL_BIGGER))
                    newBallSizeType = EBallSize.LARGE;
                if (powerUpType.Equals(EPowerUp.BALL_SMALLER))
                    newBallSizeType = EBallSize.SMALL;
                break;
            case EBallSize.SMALL:
                if (powerUpType.Equals(EPowerUp.BALL_BIGGER))
                    newBallSizeType = EBallSize.MEDIUM;
                if (powerUpType.Equals(EPowerUp.BALL_SMALLER))
                    newBallSizeType = EBallSize.XSMALL;
                break;
            case EBallSize.XSMALL:
                if (powerUpType.Equals(EPowerUp.BALL_BIGGER))
                    newBallSizeType = EBallSize.SMALL;
                break;
        }

        BallSizeSO newBallSizeSO = ballSizes.Find(e => e.BallSizeType.Equals(newBallSizeType));
        if (newBallSizeSO == null)
        {
            Debug.LogError($"[BallsManager / {name}] Cannot find BallSizeSO from its type {newBallSizeType}");
            return null;
        }

        return newBallSizeSO;
    }
    
}
