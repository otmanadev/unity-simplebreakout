using UnityEngine;

[CreateAssetMenu(fileName = "PlatformSizeSO", menuName = "Custom Objects/Platform Size")]
public class PlatformSizeSO : ScriptableObject
{
    
    [SerializeField] private EPlatformSize platformSizeType;
    public EPlatformSize PlatformSizeType => platformSizeType;

    [SerializeField] private float speed;
    public float Speed => speed;
    
    [SerializeField] private float smoothTime;
    public float SmoothTime => smoothTime;
    
    [SerializeField] private float colliderHorizontalSize;
    public float ColliderHorizontalSize => colliderHorizontalSize;
    
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
    
}