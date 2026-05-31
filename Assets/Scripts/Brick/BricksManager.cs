using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BricksManager : MonoBehaviour
{
    
    public static BricksManager Instance;

    private int _bricksCount;
    private readonly List<Brick> _allBricks = new();
    public List<Brick> AllBricks => _allBricks;
    private readonly List<Brick> _spawnedBricks = new();
    private readonly List<Brick> _aliveBricks = new();

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
    }

    /// <summary>
    /// Receive notification from single brick when instanciated.
    /// </summary>
    /// <param name="brick"></param>
    public void OnBrickInitializedNotification(Brick brick)
    {
        if (_allBricks.Contains(brick))
        {
            Debug.LogWarning($"[<color=orange>BricksManager / {name}</color>] Received notification from Brick {brick.name} but was already initialized");
            return;
        }
        
        _allBricks.Add(brick);
        VerifyIfAllBricksAreInstanciatedBeforeNotifyLevelManager();
    }

    /// <summary>
    /// Spawn all bricks.
    /// </summary>
    public void SpawnAllBricks()
    {
        foreach (Brick brick in AllBricks)
        {
            brick.StartBrickSpawn();
        }
    }

    /// <summary>
    /// Receive notification from single brick when spawned.
    /// </summary>
    /// <param name="brick"></param>
    public void OnBrickSpawnedNotification(Brick brick)
    {
        if (_spawnedBricks.Contains(brick))
        {
            Debug.LogWarning($"[<color=orange>BricksManager / {name}</color>] Received notification from Brick {brick.name} but was already spawned");
            return;
        }
        
        _spawnedBricks.Add(brick);
        VerifyIfAllBricksAreSpawnedBeforeNotifyLevelManager();
    }

    /// <summary>
    /// Verify if all bricks are instanciated before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllBricksAreInstanciatedBeforeNotifyLevelManager()
    {
        if (_allBricks.Count != _bricksCount)
            return;
        
        Debug.Log($"[<color=orange>BricksManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnBricksManagerSuccesfullyNotified();
    }

    /// <summary>
    /// Verify if all bricks are spawned before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllBricksAreSpawnedBeforeNotifyLevelManager()
    {
        if (_spawnedBricks.Count != _bricksCount)
            return;
        
        Debug.Log($"[<color=orange>BricksManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnBricksManagerSuccesfullyNotified();
    }

    public void NotifyBricksPassives()
    {
        foreach (Brick brick in AllBricks)
        {
            brick.NotifyBrickPassives();
        }
    }

    /// <summary>
    /// Receive notification from brick when he is destroyed.
    /// </summary>
    /// <param name="brick"></param>
    public void OnBrickDestroyedNotification(Brick brick)
    {
        /*
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
        */
    }
    
}
