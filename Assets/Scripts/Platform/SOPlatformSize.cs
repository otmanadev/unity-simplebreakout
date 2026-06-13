using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSize", menuName = "Custom Objects/Platform Size")]
public class SOPlatformSize : ScriptableObject
{
    
    [SerializeField] private EPlatformSize platformSize;
    public EPlatformSize PlatformSize => platformSize;

    [SerializeField, Range(-2, 2)] private int sizeLevel;
    public int SizeLevel => sizeLevel;
    
    [Header("Movement")]
    [SerializeField] private float speed;
    public float Speed => speed;
    
    [SerializeField] private float smoothTime;
    public float SmoothTime => smoothTime;
    
    [Header("Collider")]
    [SerializeField] private Vector2 colliderSize;
    public Vector2 ColliderSize => colliderSize;
    
}