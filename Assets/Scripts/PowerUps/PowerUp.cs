using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    
    [SerializeField] private EPowerUp type;
    public EPowerUp Type { get => type; set => type = value; }
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;

    private float _refZeroVelocity = .0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ThrowPowerUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var collidedObject = other.gameObject;

        if (collidedObject.GetComponent<Platform>() == null)
        {
            return;
        }
        
        Debug.Log($"[PowerUp / {name}] Send notification to {PowerUpsManager.Instance.name} : Activate power up {Type}.");
        PowerUpsManager.Instance.ActivatePowerUp(Type);
        Destroy(gameObject);
    }

    private void ThrowPowerUp()
    {
        var currentVelocityY = _rigidBody.linearVelocityY;
        var targetVelocityY = -speed;
        _rigidBody.linearVelocityY = Mathf.SmoothDamp(currentVelocityY, targetVelocityY, ref _refZeroVelocity, .0f);
    }
    
}
