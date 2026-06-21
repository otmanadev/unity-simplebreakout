using System;
using NUnit.Framework;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance;

    private ELevelState _levelState;
    public ELevelState LevelState => _levelState;

    [SerializeField] private LevelSequence startLevelSequence;
    [SerializeField] private LevelSequence endLevelSequence;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>LevelManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(startLevelSequence);
        Assert.IsNotNull(endLevelSequence);
    }

    private void Start()
    {
        UpdateLevelState(ELevelState.StartSequences);
    }

    /// <summary>
    /// Mise à jour de l'état du niveau à l'état suivant, en y ajoutant la logique pour chacun des états.
    /// </summary>
    /// <param name="newLevelState"></param>
    private void UpdateLevelState(ELevelState newLevelState)
    {
        
        Debug.Log($"[<color=orange>LevelManager / {name}</color>] Level State changed from <color=red>{_levelState}</color> to <color=green>{newLevelState}</color>");
        _levelState = newLevelState;

        switch (newLevelState)
        {
            case ELevelState.LoadingObjects:
                break;
            case ELevelState.StartSequences:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                PowerUpsManager.Instance.LinkPowerUpsToBricks();
                startLevelSequence.StartSequence();
                break;
            case ELevelState.GameStarted:
                BallsManager.Instance.StartMoveBalls();
                PlatformsManager.Instance.StartMovePlatforms();
                BricksManager.Instance.NotifyBricksPassives();
                break;
            case ELevelState.EndSequences:
                endLevelSequence.StartSequence();
                break;
            default:
                break;
        }
    }
    
    /// <summary>
    /// Reçoit une notification de la séquence pour indiquer qu'il a été terminé.
    /// </summary>
    public void OnSequenceFinishedNotification()
    {
        Debug.Log($"[LevelManager] Steps Sequence finished");
        if (_levelState.Equals(ELevelState.StartSequences))
        {
            UpdateLevelState(ELevelState.GameStarted);
            return;
        }
        
        Debug.Break();
    }

    /// <summary>
    /// Receive notification from BricksManager when no more bricks is up.
    /// </summary>
    public void OnNoMoreBricksNotification()
    {
        Debug.Log($"[<color=orange>LevelManager / {name}</color>] Receive notification from {BricksManager.Instance.name} : No more bricks to destroy.");
        UpdateLevelState(ELevelState.EndSequences);
    }

    public void OnBallReachedDeadZoneNotification()
    {
        Debug.Log($"[<color=orange>LevelManager / {name}</color>] Receive notification from {BallsManager.Instance.name} : Ball reached dead zone.");
        UpdateLevelState(ELevelState.EndSequences);
    }
    
}
