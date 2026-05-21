using UnityEngine;

[CreateAssetMenu(fileName = "BallSize", menuName = "Custom Objects/Ball Size")]
public class SOBallSize : ScriptableObject
{

    [SerializeField] private EBallSize ballSize;
    public EBallSize BallSize => ballSize;
    
    [Header("Damage")]
    [SerializeField, Min(1)] private int damage;
    public int Damage => damage;
    
    [Header("Movement")]
    [SerializeField] private float speed;
    public float Speed => speed;
    
    [Header("Collider")]
    [SerializeField] private float colliderRadius;
    public float ColliderRadius => colliderRadius;

}