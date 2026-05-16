using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSize", menuName = "Custom Objects/Platform Size")]
public class SOPlatformSize : ScriptableObject
{
    
    [SerializeField] private EPlatformSize platformSize;
    public EPlatformSize PlatformSize => platformSize;
    
    [Header("Movement")]
    [SerializeField] private float speed;
    public float Speed => speed;
    
    [SerializeField] private float smoothTime;
    public float SmoothTime => smoothTime;
    
    [Header("Sprite")][SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
    
    [Header("Collider")]
    [SerializeField] private float colliderHorizontalSize;
    public float ColliderHorizontalSize => colliderHorizontalSize;
    
}