using System.Collections.Generic;
using UnityEngine;

public class BricksManager : MonoBehaviour
{
    
    public static BricksManager Instance;
    
    private List<Brick> _bricks;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[BricksManager / {name}] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _bricks = new List<Brick>();
    }

    /// <summary>
    /// Receive notification from brick when he is created.
    /// </summary>
    /// <param name="brick"></param>
    public void OnBrickCreatedNotification(Brick brick)
    {
        if (_bricks.Contains(brick))
        {
            return;
        }
        
        _bricks.Add(brick);
        Debug.Log($"[BricksManager / {name}] Received notification from {brick.name} : Brick created.");
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
        Debug.Log($"[BricksManager / {name}] Received notification from {brick.name} : Brick destroyed.");

        if (_bricks.Count > 0)
        {
            return;
        }
        
        Debug.Log($"[BricksManager / {name}] Send notification to {GameManager.Instance.name} : No more bricks to destroy.");
        GameManager.Instance.OnNoMoreBricksNotification();
    }
    
}
