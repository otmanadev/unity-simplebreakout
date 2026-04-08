using UnityEngine;

[CreateAssetMenu(fileName = "BallSizeSO", menuName = "Custom Objects/Ball Size")]
public class BallSizeSO : ScriptableObject
{

    [SerializeField] private EBallSize ballSizeType;
    public EBallSize BallSizeType => ballSizeType;

    [SerializeField] private float speed;
    public float Speed => speed;
    
    [SerializeField] private float smoothTime;
    public float SmoothTime => smoothTime;
    
    [SerializeField] private float colliderRadius;
    public float ColliderRadius => colliderRadius;
    
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

}