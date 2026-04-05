using System;
using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{

    public static BallsManager Instance;
    
    private List<Ball> _balls;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[BallsManager / {name}] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
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
    
}
