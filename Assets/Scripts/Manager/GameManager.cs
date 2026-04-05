using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[GameManager / {name}] Instance is not unique : this instance will not be created");
            return;
        }
        Instance = this;
    }
    
}
