using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Platform : MonoBehaviour
{
    
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    
    [Header("Platform Properties")]
    [SerializeField] private PlatformSizeSO platformSizeSo;
    public PlatformSizeSO PlatformSizeSo => platformSizeSo;

    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    private float _inputHorizontalDirection = .0f;
    private float _refZeroVelocity = .0f;
    private bool _canMoveToRight = true;
    private bool _canMoveToLeft = true;
    
    private float yPosition;
    
    public float InputHorizontalDirection { set => _inputHorizontalDirection = value; }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(PlatformSizeSo);
        
        yPosition = transform.position.y;
    }
    
    private void Start()
    {
        UpdatePlatformSize();
        Debug.Log($"[Platform / {name}] Send notification to {PlatformsManager.Instance.name} : Platform created.");
        PlatformsManager.Instance.OnPlatformCreatedNotification(this);
    }

    private void FixedUpdate()
    {
        MovePlatform();
        FixVerticalVelocity();
    }
    
    /// <summary>
    /// Update platform size.
    /// </summary>
    private void UpdatePlatformSize()
    {
        _spriteRenderer.sprite = PlatformSizeSo.Sprite;
        _boxCollider.size = new Vector2(PlatformSizeSo.ColliderHorizontalSize, _boxCollider.size.y);
        speed = PlatformSizeSo.Speed;
        smoothTimeSpeed = PlatformSizeSo.SmoothTime;
    }

    /// <summary>
    /// Returns normalized direction from given position.
    /// For example : the ball collides with the platform and needs to be sent back.
    /// </summary>
    /// <param name="givenPosition"></param>
    /// <returns></returns>
    public Vector2 GetNormalizedDirection(Vector2 givenPosition)
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        return (givenPosition - currentPosition).normalized;
    }

    /// <summary>
    /// Update platform's velocity, based on player inputs.
    /// </summary>
    private void MovePlatform()
    {
        float currentInputHorizontalDirection = _inputHorizontalDirection;

        if ((currentInputHorizontalDirection < .0f && !_canMoveToLeft) || (currentInputHorizontalDirection > .0f && !_canMoveToRight))
        {
            currentInputHorizontalDirection = .0f;
        }
        
        float currentHorizontalVelocity = _rigidBody.linearVelocityX;
        float targetHorizontalVelocity = currentInputHorizontalDirection * speed;
        _rigidBody.linearVelocityX = Mathf.SmoothDamp(currentHorizontalVelocity, targetHorizontalVelocity, ref _refZeroVelocity, smoothTimeSpeed);
    }

    private void FixVerticalVelocity()
    {
        _rigidBody.linearVelocityY = .0f;
        transform.position = new Vector2(transform.position.x, yPosition);
    }
    
    /// <summary>
    /// Update new platform size.
    /// </summary>
    /// <param name="newPlatformSizeSo"></param>
    public void SetUpNewPlatformSize(PlatformSizeSO newPlatformSizeSo)
    {
        if (newPlatformSizeSo == null || newPlatformSizeSo.PlatformSizeType.Equals(PlatformSizeSo.PlatformSizeType))
        {
            return;
        }
        
        platformSizeSo = newPlatformSizeSo;
        UpdatePlatformSize();
    }
    
}
