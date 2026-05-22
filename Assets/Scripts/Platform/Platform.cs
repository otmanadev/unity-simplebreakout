using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Platform : MonoBehaviour
{
    
    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    private static readonly String AnimationTriggerPlatformBigger = "BiggerPlatformTrigger";
    private static readonly String AnimationTriggerPlatformSmaller = "SmallerPlatformTrigger";
    
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    
    [SerializeField] private EPlatformSize platformSize;
    public EPlatformSize PlatformSize => platformSize;

    [Header("Movement Properties")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    
    private float _inputHorizontalDirection = .0f;
    public float InputHorizontalDirection { set => _inputHorizontalDirection = value; }
    private float _refZeroVelocity = .0f;
    private float _yPosition;

    [Header("Ball Direction Properties")] 
    [SerializeField, Min(.0f)] private float maxAngleOnCorners;

    [Header("Ball direction preview properties")]
    [SerializeField] private GameObject ballPreviewPrefab;
    [SerializeField, Min(.0f)] private float ballDistanceToShowPreviewBalls;
    [SerializeField, Min(0L)] private int numberOfBallPreviews;
    [SerializeField, Min(.0f)] private float minDistanceForBallDirectionPreview;
    [SerializeField, Min(.0f)] private float ballPreviewDistance;
    private Dictionary<int, List<GameObject>> _ballPreviewsInstances;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        
        _yPosition = transform.position.y;
        Assert.IsNotNull(ballPreviewPrefab);

        _ballPreviewsInstances = new Dictionary<int, List<GameObject>>();
    }
    
    private void Start()
    {
        UpdatePlatformSize(PlatformSize);
        InitializeBallPreviewInstancesForEachBall();
        
        Debug.Log($"[Platform / {name}] Send notification to Platforms Manager : <color=orange>Platform initialized</color>");
        PlatformsManager.Instance.OnPlatformInitializedNotification(this);
    }

    private void FixedUpdate()
    {
        MovePlatform();
        FixVerticalVelocity();
        UpdatePreviewBalls();
    }

    private void InitializeBallPreviewInstancesForEachBall()
    {
        foreach (Ball ball in BallsManager.Instance.AllBalls)
        {
            int ballId = ball.GetInstanceID();
            _ballPreviewsInstances.Add(ballId, new List<GameObject>());
            
            for (int i = 0; i < numberOfBallPreviews; i++)
            {
                Vector2 ballPreviewCoordinates = new Vector2(ball.transform.position.x,
                    transform.position.y + minDistanceForBallDirectionPreview + i * ballPreviewDistance);
                
                GameObject ballPreviewInstance = ballPreviewPrefab;
                ballPreviewInstance.SetActive(false);
                
                Color ballPreviewColor = ballPreviewInstance.GetComponent<SpriteRenderer>().color;
                ballPreviewColor.a = (numberOfBallPreviews + 1f - (i + 1f)) / numberOfBallPreviews;
                ballPreviewInstance.GetComponent<SpriteRenderer>().color = ballPreviewColor;
                
                GameObject ballPreviewCreatedInstance = Instantiate(ballPreviewInstance, ballPreviewCoordinates, Quaternion.identity);
                _ballPreviewsInstances[ballId].Add(ballPreviewCreatedInstance);
            }
            
            Debug.Log($"<color=red>Added ball {ballId} with {_ballPreviewsInstances[ballId].Count} preview balls</color>");
        }
    }
    
    /// <summary>
    /// Play platform spawn animation.
    /// </summary>
    public void SpawnPlatform()
    {
        Debug.Log($"[Platform / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }

    /// <summary>
    /// Notify Platforms Manager the platform finished his spawn animation.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorPlatformSpawned()
    {
        Debug.Log($"[Platform / {name}] Send notification to Platforms Manager : <color=orange>Platform spawned</color>");
        PlatformsManager.Instance.OnPlatformSpawnedNotification(this);
    }
    
    /// <summary>
    /// Update platform size.
    /// </summary>
    /// <param name="newPlatformSize"></param>
    private void UpdatePlatformSize(EPlatformSize newPlatformSize)
    {
        platformSize = newPlatformSize;
        SOPlatformSize platformSizeProperties = PlatformsManager.Instance.GetPlatformSizeByItsType(platformSize);

        _boxCollider.size = platformSizeProperties.ColliderSize;
        speed = platformSizeProperties.Speed;
        smoothTimeSpeed = platformSizeProperties.SmoothTime;
    }

    /// <summary>
    /// Returns normalized direction from given position (x).
    /// For example : the ball collides with the platform and needs a reflection.
    /// </summary>
    /// <param name="givenPosition"></param>
    /// <returns></returns>
    public Vector2 GetBallNormalizedDirectionFromGivenPosition(float givenPosition)
    {
        float minCornerPosition = transform.position.x - _boxCollider.size.x / 2;
        float maxCornerPosition = transform.position.x + _boxCollider.size.x / 2;
        
        // On garde la coordonnée dans les bornes
        float clampedGivenPosition = Mathf.Clamp(givenPosition, 
            minCornerPosition,
            maxCornerPosition);

        // Renvoie une valeur entre 0 et 1
        float t = Mathf.InverseLerp(minCornerPosition, maxCornerPosition, clampedGivenPosition);

        // Renvoie l'angle à renvoyer
        float angle = Mathf.Lerp(-maxAngleOnCorners, maxAngleOnCorners, t);
        
        // On part de Vector2.up, il faut donc inverser
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Sin(radians),
            Mathf.Cos(radians)
        );
    }

    /// <summary>
    /// Update platform's velocity, based on player inputs.
    /// </summary>
    private void MovePlatform()
    {
        float currentHorizontalVelocity = _rigidBody.linearVelocityX;
        float targetHorizontalVelocity = _inputHorizontalDirection * speed;
        _rigidBody.linearVelocityX = Mathf.SmoothDamp(currentHorizontalVelocity, targetHorizontalVelocity, ref _refZeroVelocity, smoothTimeSpeed);
    }

    private void FixVerticalVelocity()
    {
        _rigidBody.linearVelocityY = .0f;
        transform.position = new Vector2(transform.position.x, _yPosition);
    }
    
    /// <summary>
    /// Update new platform size.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="newPlatformSizeType"></param>
    public void SetUpNewPlatformSize(EPowerUp powerUpType, EPlatformSize newPlatformSizeType)
    {
        if (PlatformSize.Equals(newPlatformSizeType))
            return;
        
        switch (powerUpType)
        {
            case EPowerUp.PlatformBigger:
                _animator.SetTrigger(AnimationTriggerPlatformBigger);
                break;
            case EPowerUp.PlatformSmaller:
                _animator.SetTrigger(AnimationTriggerPlatformSmaller);
                break;
        }
        
        UpdatePlatformSize(newPlatformSizeType);
    }
    
    private void UpdatePreviewBalls()
    {
        float minX = transform.position.x - _boxCollider.size.x / 2;
        float maxX = transform.position.x + _boxCollider.size.x / 2;

        foreach (Ball ball in BallsManager.Instance.AllBalls)
        {
            float distance = Vector2.Distance(Vector2.up * ball.transform.position.y, Vector2.up * transform.position.y);
            if (!LevelManager.Instance.LevelState.Equals(ELevelState.ActivePhase)
                || ball.transform.position.x < minX 
                || ball.transform.position.x > maxX 
                || distance > ballDistanceToShowPreviewBalls 
                || ball.Direction.y >= .0f)
            {
                UpdatePreviewBallsWithVisibility(ball, false);
                continue;
            }

            UpdatePreviewBallsWithVisibility(ball, true);
        }
    }

    private void UpdatePreviewBallsWithVisibility(Ball ball, bool visible)
    {
        int ballId = ball.GetInstanceID();
        if (!_ballPreviewsInstances.TryGetValue(ballId, out List<GameObject> ballPreviews))
            return;

        Vector2 origin = ball.transform.position;
        Vector2 direction = GetBallNormalizedDirectionFromGivenPosition(origin.x);
        foreach (GameObject previewBallInstance in ballPreviews)
        {
            // Calcul de la coordonnée X en fonction du point d'origine, la direction et la coordonnée Y connue.
            float targetY = previewBallInstance.transform.position.y;
            float t = (targetY - origin.y) / direction.y;
            float targetX = origin.x + t * direction.x;
            Vector2 ballPreviewCoordinates = new Vector2(targetX, targetY);
            
            previewBallInstance.transform.position = ballPreviewCoordinates; 
            previewBallInstance.SetActive(visible);
        }
    }
    
}
