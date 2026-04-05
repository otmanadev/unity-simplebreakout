using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[GameManager / {name}] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Receive notification from BricksManager when no more bricks is up.
    /// </summary>
    public void OnNoMoreBricksNotification()
    {
        Debug.Log($"[GameManager / {name}] Receive notification from {BricksManager.Instance.name} : No more bricks to destroy.");
        Debug.Break();
    }
    
}
