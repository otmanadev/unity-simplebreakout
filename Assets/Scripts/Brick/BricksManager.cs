using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BricksManager : Manager
{
    
    public static BricksManager Instance;

    [SerializeField] private List<Brick> _allBricks;
    public List<Brick> AllBricks => _allBricks;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>BricksManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _allBricks = FindObjectsByType<Brick>(FindObjectsSortMode.InstanceID).ToList();
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
        if (!AllBricks.Contains(brick))
            return;
        
        _allBricks.Remove(brick);

        if (AllBricks.Count > 0)
            return;
        
        LevelManager.Instance.OnNoMoreBricksNotification();
    }
    
}
