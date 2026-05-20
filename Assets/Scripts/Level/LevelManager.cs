using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance;

    private ELevelState _levelState;

    private bool _bricksManagerNotificationReceived;
    private bool _powerUpsManagerNotificationReceived;
    private bool _ballsManagerNotificationReceived;
    private bool _platformsManagerNotificationReceived;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>LevelManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        UpdateLevelState(ELevelState.LoadingObjects);
    }

    /// <summary>
    /// Update Level State. Each State might have different behaviours. See details on the documentation or while reading the code.
    /// </summary>
    /// <param name="newLevelState"></param>
    private void UpdateLevelState(ELevelState newLevelState)
    {
        _bricksManagerNotificationReceived = false;
        _ballsManagerNotificationReceived = false;
        _platformsManagerNotificationReceived = false;
        
        Debug.Log($"[<color=orange>LevelManager / {name}</color>] Level State changed from <color=red>{_levelState}</color> to <color=green>{newLevelState}</color>");
        _levelState = newLevelState;

        switch (newLevelState)
        {
            case ELevelState.LoadingObjects:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case ELevelState.AppearingObjects:
                BricksManager.Instance.SpawnAllBricks();
                BallsManager.Instance.SpawnAllBalls();
                PlatformsManager.Instance.SpawnAllPlatforms();
                break;
            case ELevelState.ActivePhase:
                BallsManager.Instance.StartMoveBalls();
                PlatformsManager.Instance.StartMovePlatforms();
                break;
            default:
                break;
        }
    }
    
    /// <summary>
    /// Verify if all managers sent notification so it can update Level State.
    /// </summary>
    private void CheckIfStateCanBeUpdated()
    {
        if (!_bricksManagerNotificationReceived || !_ballsManagerNotificationReceived || !_platformsManagerNotificationReceived || !_powerUpsManagerNotificationReceived)
            return;

        switch (_levelState)
        {
            case ELevelState.LoadingObjects:
                UpdateLevelState(ELevelState.AppearingObjects);
                break;
            case ELevelState.AppearingObjects:
                UpdateLevelState(ELevelState.ActivePhase);
                break;
            case ELevelState.ActivePhase:
                UpdateLevelState(ELevelState.VanishingObjects);
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// Receive notification from Bricks Manager, so that the game can update his state.
    /// </summary>
    public void OnBricksManagerSuccesfullyNotified()
    {
        if (_bricksManagerNotificationReceived)
        {
            Debug.LogWarning($"[<color=orange>LevelManager / {name}</color>] Bricks Manager notification already received at Level State <color=orange>{_levelState}</color>");
            return;
        }

        if (_levelState.Equals(ELevelState.LoadingObjects))
        {
            PowerUpsManager.Instance.LinkPowerUpsToBricks();
        }

        _bricksManagerNotificationReceived = true;
        CheckIfStateCanBeUpdated();
    }
    
    /// <summary>
    /// Receive notification from power Ups Manager, so that the game can update his state.
    /// </summary>
    public void OnPowerUpsManagerSuccesfullyNotified()
    {
        if (_powerUpsManagerNotificationReceived)
        {
            Debug.LogWarning($"[<color=orange>LevelManager / {name}</color>] Bricks Manager notification already received at Level State <color=orange>{_levelState}</color>");
            return;
        }

        _powerUpsManagerNotificationReceived = true;
        CheckIfStateCanBeUpdated();
    }
    
    /// <summary>
    /// Receive notification from Balls Manager, so that the game can update his state.
    /// </summary>
    public void OnBallsManagerSuccesfullyNotified()
    {
        if (_ballsManagerNotificationReceived)
        {
            Debug.LogWarning($"[<color=orange>LevelManager / {name}</color>] Balls Manager notification already received at Level State <color=orange>{_levelState}</color>");
            return;
        }

        _ballsManagerNotificationReceived = true;
        CheckIfStateCanBeUpdated();
    }
    
    /// <summary>
    /// Receive notification from Platforms Manager, so that the game can update his state.
    /// </summary>
    public void OnPlatformsManagerSuccesfullyNotified()
    {
        if (_platformsManagerNotificationReceived)
        {
            Debug.LogWarning($"[<color=orange>LevelManager / {name}</color>] Platforms Manager notification already received at Level State <color=orange>{_levelState}</color>");
            return;
        }

        _platformsManagerNotificationReceived = true;
        CheckIfStateCanBeUpdated();
    }

    /// <summary>
    /// Receive notification from BricksManager when no more bricks is up.
    /// </summary>
    public void OnNoMoreBricksNotification()
    {
        Debug.Log($"[<color=orange>LevelManager / {name}</color>] Receive notification from {BricksManager.Instance.name} : No more bricks to destroy.");
        Debug.Break();
    }

    public void OnBallReachedDeadZoneNotification()
    {
        Debug.Log($"[<color=orange>LevelManager / {name}</color>] Receive notification from {BallsManager.Instance.name} : Ball reached dead zone.");
        Debug.Break();
    }
    
}
