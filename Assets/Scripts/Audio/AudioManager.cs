using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>AudioManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
}
