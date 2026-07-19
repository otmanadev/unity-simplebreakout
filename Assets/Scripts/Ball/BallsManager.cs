using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BallsManager : Manager
{

    public static BallsManager Instance;

    [Header("Balls")]
    [SerializeField] private List<Ball> _allBalls;
    public List<Ball> AllBalls => _allBalls;
    
    [Header("Ball movement fade properties")] 
    [SerializeField, Min(.0f)] private float movementFadeDuration;
    [Range(0f, 1f)] private float _movementMultiplier = .0f;
    public float MovementMultiplier => _movementMultiplier;
    private float _currentMovementFadeDuration = .0f;
    private bool _fadeInProgress = false;

    [Header("Metadatas : Ball Sizes")] 
    [SerializeField] private SOBallSize extraLargeBall;
    [SerializeField] private SOBallSize largeBall;
    [SerializeField] private SOBallSize mediumBall;
    [SerializeField] private SOBallSize smallBall;
    [SerializeField] private SOBallSize extraSmallBall;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>BallsManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Ball sizes
        Assert.IsNotNull(extraLargeBall);
        Assert.IsTrue(extraLargeBall.BallSize.Equals(EBallSize.ExtraLarge));
        
        Assert.IsNotNull(largeBall);
        Assert.IsTrue(largeBall.BallSize.Equals(EBallSize.Large));
        
        Assert.IsNotNull(mediumBall);
        Assert.IsTrue(mediumBall.BallSize.Equals(EBallSize.Medium));
        
        Assert.IsNotNull(smallBall);
        Assert.IsTrue(smallBall.BallSize.Equals(EBallSize.Small));
        
        Assert.IsNotNull(extraSmallBall);
        Assert.IsTrue(extraSmallBall.BallSize.Equals(EBallSize.ExtraSmall));
        
        _allBalls = FindObjectsByType<Ball>(FindObjectsSortMode.InstanceID).ToList();
    }

    private void Update()
    {
        UpdateMovementMultiplier();
    }
    
    private void UpdateMovementMultiplier()
    {
        if (!_fadeInProgress) return;

        _currentMovementFadeDuration += Time.deltaTime;
        
        if (_currentMovementFadeDuration >= movementFadeDuration)
        {
            _currentMovementFadeDuration = .0f;
            _movementMultiplier = 1.0f;
            _fadeInProgress = false;
            return;
        }

        _movementMultiplier = _currentMovementFadeDuration / movementFadeDuration;
    }
    
    /// <summary>
    /// Start move balls.
    /// </summary>
    public void StartMoveBalls()
    {
        foreach (Ball ball in AllBalls)
        {
            ball.StartMoveBall();
        }
        _fadeInProgress = true;
    }
    
    /// <summary>
    /// Receive notification from ball when he is created.
    /// </summary>
    /// <param name="ball"></param>
    public void OnBallReachDeadZoneNotification(Ball ball)
    {
        Debug.Log($"[<color=orange>BallsManager / {name}</color>] Received notification from {ball.name} : Ball reached dead zone.");
        Debug.Log($"[<color=orange>BallsManager / {name}</color>] Send notification from {LevelManager.Instance.name} : Ball reached dead zone.");
        LevelManager.Instance.OnBallReachedDeadZoneNotification();
    }

    /// <summary>
    /// Enable given power up for every balls.
    /// </summary>
    /// <param name="powerUpType"></param>
    public void EnablePickedPowerUp(EPowerUp powerUpType)
    {
        foreach (Ball ball in _allBalls)
        {
            switch (powerUpType)
            {
                case EPowerUp.BallBigger:
                case EPowerUp.BallSmaller:
                    EBallSize currentBallSizeType = ball.BallSize;
                    EBallSize newBallSizeType = GetNewBallSizePropertiesFromPowerUp(powerUpType, currentBallSizeType);
                    ball.SetUpNewBallSize(powerUpType, newBallSizeType);
                    break;
            }
        }
    }

    private EBallSize GetNewBallSizePropertiesFromPowerUp(EPowerUp powerUpType, EBallSize currentBallSizeType)
    {
        switch (currentBallSizeType)
        {
            case EBallSize.ExtraLarge:
                if (powerUpType.Equals(EPowerUp.BallSmaller))
                    return EBallSize.Large;
                break;
            case EBallSize.Large:
                if (powerUpType.Equals(EPowerUp.BallBigger))
                    return EBallSize.ExtraLarge;
                if (powerUpType.Equals(EPowerUp.BallSmaller))
                    return EBallSize.Medium;
                break;
            case EBallSize.Medium:
                if (powerUpType.Equals(EPowerUp.BallBigger))
                    return EBallSize.Large;
                if (powerUpType.Equals(EPowerUp.BallSmaller))
                    return EBallSize.Small;
                break;
            case EBallSize.Small:
                if (powerUpType.Equals(EPowerUp.BallBigger))
                    return EBallSize.Medium;
                if (powerUpType.Equals(EPowerUp.BallSmaller))
                    return EBallSize.ExtraSmall;
                break;
            case EBallSize.ExtraSmall:
                if (powerUpType.Equals(EPowerUp.BallBigger))
                    return EBallSize.Large;
                break;
        }

        return currentBallSizeType;
    }

    /// <summary>
    /// Return right ball size properties depending on its type.
    /// </summary>
    /// <param name="ballSizeType"></param>
    /// <returns></returns>
    public SOBallSize GetBallSizeByItsType(EBallSize ballSizeType)
    {
        switch (ballSizeType)
        {
            case EBallSize.ExtraLarge:
                return extraLargeBall;
            case EBallSize.Large:
                return largeBall;
            case EBallSize.Medium:
                return mediumBall;;
            case EBallSize.Small:
                return smallBall;
            case EBallSize.ExtraSmall:
                return extraSmallBall;
            default:
                return mediumBall;
        }
    }
    
}
