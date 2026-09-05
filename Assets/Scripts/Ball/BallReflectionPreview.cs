using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Ball))]
public class BallReflectionPreview : MonoBehaviour
{

    private Ball _ball;
    private readonly List<GameObject> _ballReflectionPreviewInstances = new();
    
    [Header("Main Properties")]
    [SerializeField] private GameObject ballReflectionPreviewPrefab;
    [SerializeField, Min(1)] private int numberOfBallPreviews;
    
    [Header("Distance Properties")]
    [SerializeField, Min(.0f)] private float ballDistanceToShowPreviewBalls;
    [SerializeField, Min(.0f)] private float distanceBetweenPlatformAndFirstBallPreviewInstance;
    [SerializeField, Min(.0f)] private float distanceBetweenTwoBallPreviewInstances;
    
    [Header("Color Properties")]
    [SerializeField, Min(.0f)] private float alphaMin;
    [SerializeField, Min(.0f)] private float alphaMax;

    private void Awake()
    {
        Assert.IsNotNull(ballReflectionPreviewPrefab);
        
        _ball = GetComponent<Ball>();
    }

    private void FixedUpdate()
    {
        PreviewBall();
    }

    private void PreviewBall()
    {
        RaycastHit2D? nextPlatformForBallReflectionPreview = 
            GetClosestPlatformForBallReflectionPreview();
        if (HasRaycastHit2DCollisionWithPlatform(nextPlatformForBallReflectionPreview))
        {
            ClearCurrentBallPreviews();
            return;
        }

        ShowBallPreviews(nextPlatformForBallReflectionPreview.Value);
    }
    
    /// <summary>
    /// Vérifie qu'on entre dans les conditions pour faire afficher les BallReflectionPreview :
    /// - Distance ;
    /// - La balle descend ;
    /// - La direction de la balle et la distance rencontre une plateforme.
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D? GetClosestPlatformForBallReflectionPreview()
    {
        Vector2 ballDirection = _ball.Movement.Direction;
        if (ballDirection.y >= .0f)
            return null;
        
        return Physics2D.RaycastAll(transform.position, ballDirection, ballDistanceToShowPreviewBalls)
            .ToList()
            .Where(hit2D => hit2D.collider.TryGetComponent(out Platform _))
            .OrderBy(hit2D => hit2D.distance)
            .FirstOrDefault(); 
    }

    private bool HasRaycastHit2DCollisionWithPlatform(RaycastHit2D? raycastHit2D)
    {
        return raycastHit2D is null || raycastHit2D.Value.collider is null;
    }

    /// <summary>
    /// Détruit les BallPreview existants s'il y en a de référencé.
    /// </summary>
    private void ClearCurrentBallPreviews()
    {
        if (_ballReflectionPreviewInstances.Count == 0)
            return;

        foreach (GameObject instance in _ballReflectionPreviewInstances)
        {
            Destroy(instance);
        }
        
        _ballReflectionPreviewInstances.Clear();
    }

    /// <summary>
    /// Fait apparaitre ou met à jour les instances de BallPreview à partir de notre position de départ proche de la Plateforme.
    /// </summary>
    /// <param name="raycastHit2D"></param>
    private void ShowBallPreviews(RaycastHit2D raycastHit2D)
    {
        Vector2 collisionPoint = raycastHit2D.point;

        raycastHit2D.collider.TryGetComponent(out Platform platform);
        Vector2 originPlatformPoint = platform.GetStartPointForBallPreview(collisionPoint);
        
        Vector2 direction = platform.GetBallNormalizedDirectionFromGivenPosition(originPlatformPoint.x);

        if (_ballReflectionPreviewInstances.Count == 0)
        {
            CreateInstancesOfBallPreview(originPlatformPoint, direction);
        }
        else
        {
            UpdateInstancesOfBallPreview(originPlatformPoint, direction);
        }
    }

    /// <summary>
    /// Créé les instances de BallPreview.
    /// </summary>
    /// <param name="originPlatformPoint"></param>
    /// <param name="direction"></param>
    private void CreateInstancesOfBallPreview(Vector2 originPlatformPoint, Vector2 direction)
    {
        for (int i = 0; i < numberOfBallPreviews; i++)
        {
            Vector2 ballPreviewCoordinates = GetPreviewBallCoordinates(originPlatformPoint, direction, i);
            GameObject instance = Instantiate(ballReflectionPreviewPrefab, ballPreviewCoordinates, Quaternion.identity);
            SetAlphaColorForBallPreviewInstance(instance, i);
            _ballReflectionPreviewInstances.Add(instance);
        }
    }
    
    /// <summary>
    /// Met à jour les instances de BallPreview.
    /// </summary>
    /// <param name="originPlatformPoint"></param>
    /// <param name="direction"></param>
    private void UpdateInstancesOfBallPreview(Vector2 originPlatformPoint, Vector2 direction)
    {
        for (int i = 0; i < numberOfBallPreviews; i++)
        {
            GameObject instance = _ballReflectionPreviewInstances[i];
            Vector2 ballPreviewCoordinates = GetPreviewBallCoordinates(originPlatformPoint, direction, i);
            instance.transform.position = ballPreviewCoordinates;
        }
    }

    /// <summary>
    /// Calcule la coordonnée de la BallPreview en fonction de son index, la direction de la balle et le point de départ.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="direction"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector2 GetPreviewBallCoordinates(Vector2 startPoint, Vector2 direction, int index)
    {
        return startPoint 
               + Vector2.up * distanceBetweenPlatformAndFirstBallPreviewInstance 
               + index * distanceBetweenTwoBallPreviewInstances * direction.normalized;
    }

    /// <summary>
    /// Calcule la couleur transparente de l'instance de la BallPreview en fonction de son index.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="index"></param>
    private void SetAlphaColorForBallPreviewInstance(GameObject instance, int index)
    {
        instance.TryGetComponent(out SpriteRenderer spriteRenderer);
        Color ballPreviewColor = spriteRenderer.color;
        ballPreviewColor.a = Mathf.Lerp(alphaMin, alphaMax, (float)index / numberOfBallPreviews);
        spriteRenderer.color = ballPreviewColor;
    }
    
}