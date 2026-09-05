using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour, IPlatformTriggerHandler
{

    private Rigidbody2D _rigidBody;
    
    [SerializeField] private EPowerUp type;
    public EPowerUp Type { get => type; set => type = value; }
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;

    private float _refZeroVelocity = .0f;
    
    [Header("Pickup properties")]
    [SerializeField] private GameObject pickupAudioPrefab;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        
        Assert.IsNotNull(pickupAudioPrefab);
        Assert.IsTrue(pickupAudioPrefab.GetComponent<Audio>());
    }

    private void FixedUpdate()
    {
        ThrowPowerUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Platform platform))
            platform.RegisterTriggerEnter(this, other);
    }

    private void ThrowPowerUp()
    {
        var currentVelocityY = _rigidBody.linearVelocityY;
        var targetVelocityY = -speed;
        _rigidBody.linearVelocityY = Mathf.SmoothDamp(currentVelocityY, targetVelocityY, ref _refZeroVelocity, .0f);
    }

    public TriggerResponse HandlePlatformTriggerEnter(Collider2D _, Platform platform)
    {
        Instantiate(pickupAudioPrefab, transform.position, Quaternion.identity);
        PowerUpsManager.Instance.ActivatePowerUp(Type);
        Destroy(gameObject);
        
        return new TriggerResponse();
    }

    /// <summary>
    /// Aucune implémentation prévue à cet effet.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="__"></param>
    /// <returns></returns>
    public TriggerResponse HandlePlatformTriggerExit(Collider2D _, Platform __)
    {
        return new TriggerResponse();
    }
}
