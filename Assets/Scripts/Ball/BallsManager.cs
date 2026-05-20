using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BallsManager : MonoBehaviour
{

    public static BallsManager Instance;

    private int _ballsCount;
    private readonly List<Ball> _allBalls = new();
    private readonly List<Ball> _spawnedBalls = new();
    
    [Header("Ball movement fade properties")] 
    [SerializeField, Min(.0f)] private float movementFadeDuration;
    [Range(0f, 1f)] private float _movementMultiplier = .0f;
    public float MovementMultiplier => _movementMultiplier;
    private float _currentMovementFadeDuration = .0f;
    private bool _fadeInProgress = false;
    
    [Header("Ball Datas")]
    [SerializeField] private List<SOBallSize> ballSizes;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>BallsManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(ballSizes);
        
        _ballsCount = GameObject.FindGameObjectsWithTag("Ball")
            .Where(o => o.GetComponent<Ball>() != null)
            .Count();
    }

    private void Update()
    {
        UpdateMovementMultiplier();
    }

    /// <summary>
    /// Receive notification from ball when initialized.
    /// </summary>
    /// <param name="ball"></param>
    public void OnBallInitializedNotification(Ball ball)
    {
        if (_allBalls.Contains(ball))
        {
            Debug.LogWarning($"[<color=orange>BallsManager / {name}</color>] Received notification from Ball {ball.name} but was already initialized");
            return;
        }
        
        _allBalls.Add(ball);
        VerifyIfAllBallsAreInstanciatedBeforeNotifyLevelManager();
    }
    
    /// <summary>
    /// Verify if all balls are instanciated before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllBallsAreInstanciatedBeforeNotifyLevelManager()
    {
        if (_allBalls.Count != _ballsCount)
            return;
        
        Debug.Log($"[<color=orange>BallsManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnBallsManagerSuccesfullyNotified();
    }

    /// <summary>
    /// Spawn all balls.
    /// </summary>
    public void SpawnAllBalls()
    {
        foreach (Ball ball in _allBalls)
        {
            ball.StartBallSpawn();
        }
    }

    /// <summary>
    /// Receive notification from single ball when spawned.
    /// </summary>
    /// <param name="ball"></param>
    public void OnBallSpawnedNotification(Ball ball)
    {
        if (_spawnedBalls.Contains(ball))
        {
            Debug.LogWarning($"[<color=orange>BallsManager / {name}</color>] Received notification from Ball {ball.name} but was already spawned");
            return;
        }
        
        _spawnedBalls.Add(ball);
        VerifyIfAllBallsAreSpawnedBeforeNotifyLevelManager();
    }
    
    /// <summary>
    /// Verify if all balls are spawned before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllBallsAreSpawnedBeforeNotifyLevelManager()
    {
        if (_spawnedBalls.Count != _ballsCount)
            return;
        
        Debug.Log($"[<color=orange>BallsManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnBallsManagerSuccesfullyNotified();
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
        foreach (Ball ball in _allBalls)
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
        if (!_allBalls.Contains(ball))
        {
            return;
        }
        
        Debug.Log($"[<color=orange>BallsManager / {name}</color>] Received notification from {ball.name} : Ball reached dead zone.");
        Debug.Log($"[<color=orange>BallsManager / {name}</color>] Send notification from {LevelManager.Instance.name} : Ball reached dead zone.");
        LevelManager.Instance.OnBallReachedDeadZoneNotification();
    }

    /// <summary>
    /// Enable given power up for every balls.
    /// </summary>
    /// <param name="powerUpType"></param>
    public void ActivateBallPowerUp(EPowerUp powerUpType)
    {
        foreach (Ball ball in _allBalls)
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
            Debug.LogError($"[<color=orange>BallsManager / {name}</color>] Cannot find BallSizeSO from power up type {powerUpType}");
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
            Debug.LogError($"[<color=orange>BallsManager / {name}</color>] Cannot find BallSizeSO from size type {newBallSizeType}");
            return null;
        }

        return newSoBallSize;
    }
    
}
