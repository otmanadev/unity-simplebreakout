using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BricksManager : MonoBehaviour
{
    
    public static BricksManager Instance;

    private int _bricksCount;
    private List<Brick> _bricks;
    public List<Brick> Bricks => _bricks;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>BricksManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _bricksCount = GameObject.FindGameObjectsWithTag("Brick")
            .Where(o => o.GetComponent<Brick>() != null)
            .Count();
        _bricks = new List<Brick>();
    }

    /// <summary>
    /// Receive notification from single brick when instanciated.
    /// </summary>
    /// <param name="brick"></param>
    public void OnBrickInitializedNotification(Brick brick)
    {
        if (_bricks.Contains(brick))
        {
            Debug.LogWarning($"[<color=orange>BricksManager / {name}</color>] Received notification from Brick {brick.name} but was already initialized");
            return;
        }
        
        _bricks.Add(brick);
        VerifyIfAllBricksAreInstanciatedBeforeNotifyLevelManager();
    }

    /// <summary>
    /// Verify if all bricks are instanciated before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllBricksAreInstanciatedBeforeNotifyLevelManager()
    {
        if (_bricks.Count != _bricksCount)
            return;
        
        Debug.Log($"[<color=orange>BricksManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnBricksManagerSuccesfullyNotified();
    }

    /// <summary>
    /// Receive notification from brick when he is destroyed.
    /// </summary>
    /// <param name="brick"></param>
    public void OnBrickDestroyedNotification(Brick brick)
    {
        if (!_bricks.Contains(brick))
        {
            return;
        }
        
        _bricks.Remove(brick);
        Debug.Log($"[<color=orange>BricksManager / {name}</color>] Received notification from {brick.name} : Brick destroyed.");

        if (_bricks.Count > 0)
        {
            return;
        }
        
        Debug.Log($"[<color=orange>BricksManager / {name}</color>] Send notification to {LevelManager.Instance.name} : No more bricks to destroy.");
        LevelManager.Instance.OnNoMoreBricksNotification();
    }
    
}
