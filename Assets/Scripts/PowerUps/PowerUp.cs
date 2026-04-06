using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    
    [SerializeField] private EPowerUp powerUpType;
    
    public EPowerUp PowerUpType => powerUpType;
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;

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
        GameObject collidedObject = other.gameObject;

        if (collidedObject.GetComponent<Platform>() == null)
        {
            return;
        }
        
        Debug.Log($"[PowerUp / {name}] Activating power up {PowerUpType}...");
        Destroy(gameObject);
    }

    private void ThrowPowerUp()
    {
        float currentVelocityY = _rigidBody.linearVelocityY;
        float targetVelocityY = -speed;
        _rigidBody.linearVelocityY = Mathf.SmoothDamp(currentVelocityY, targetVelocityY, ref _refZeroVelocity, smoothTimeSpeed);
    }
    
}
