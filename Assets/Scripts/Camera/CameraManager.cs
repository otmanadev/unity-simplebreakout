using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance;
    
    private Camera _camera;
    private PixelPerfectCamera _pixelPerfectCamera;
    
    private Vector3 _cameraPosition;

    public event Action OnCameraMovementCompleted;

    [Header("Shake Properties")] 
    [SerializeField] private float _trauma;
    [SerializeField] private Vector2 _impactDirection;
    [SerializeField] private float maxOffset = 0.15f;
    [SerializeField] private float traumaDecay = 3;
    [SerializeField] private float traumaExponent = 2;
    [SerializeField] private float randomInfluence = 0.2f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one CameraManager in scene!");
            return;
        }
        Instance = this;
        
        _camera = GetComponent<Camera>();
        _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        
        _cameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        UpdateShakeCameraCoordinates();
    }

    /// <summary>
    /// Fait déplacer la caméra, et notifie une fois le déplacement terminé.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="targetPixelsPerUnit"></param>
    /// <param name="duration"></param>
    public void MoveCamera(Vector2 targetPosition, int targetPixelsPerUnit, float duration)
    {
        if (CannotMoveCamera(targetPosition, targetPixelsPerUnit, duration))
        {
            NotifyCameraMovementCompleted();
            return;
        }
        
        StartCoroutine(MoveCameraCoroutine(targetPosition, targetPixelsPerUnit, duration));
    }

    private bool CannotMoveCamera(Vector2 targetPosition, int targetPixelsPerUnit, float duration) =>
        duration <= 0.0f || 
        (targetPosition.x.Equals(_camera.transform.position.x) && targetPosition.y.Equals(_camera.transform.position.y) 
                                                               && targetPixelsPerUnit.Equals(_pixelPerfectCamera.assetsPPU));

    private IEnumerator MoveCameraCoroutine(Vector2 targetPosition, int targetPixelsPerUnit, float duration)
    {
        float elapsedTime = 0.0f;
        float t = 0.0f;
        Vector3 sourcePosition = _cameraPosition;
        int sourcePixelsPerUnit = _pixelPerfectCamera.assetsPPU;

        Debug.Log($"[CameraManager] Moving camera from {sourcePosition} (PPU {sourcePixelsPerUnit}) to {targetPosition} (PPU {targetPixelsPerUnit}) in {duration} seconds.]");
        while (elapsedTime < duration)
        {
            _cameraPosition = Vector3.Lerp(sourcePosition, 
                new Vector3(targetPosition.x, targetPosition.y, sourcePosition.z), 
                t);
            transform.position = _cameraPosition;
            
            _pixelPerfectCamera.assetsPPU = (int) Mathf.Lerp(sourcePixelsPerUnit, 
                targetPixelsPerUnit, 
                t);
            
            yield return null;
            
            elapsedTime += Time.deltaTime;
            t = Mathf.Min(Mathf.Clamp01(elapsedTime / duration), 1.0f);
        }
        
        NotifyCameraMovementCompleted();
    }

    private void NotifyCameraMovementCompleted()
    {
        OnCameraMovementCompleted?.Invoke();
    }

    /// <summary>
    /// Fait trembler la caméra avec une intensité et une direction.
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="direction"></param>
    public void Shake(float intensity, Vector2 direction)
    {
        _trauma = Mathf.Clamp01(_trauma + intensity);

        if (direction.sqrMagnitude > 0.001f)
        {
            _impactDirection = direction.normalized;
        }
    }

    private void UpdateShakeCameraCoordinates()
    {
        _trauma = Mathf.Max(
            0f,
            _trauma - traumaDecay * Time.deltaTime
        );

        float amplitude = maxOffset
                          * Mathf.Pow(_trauma, traumaExponent);

        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle;

        Vector2 offsetDirection =
            _impactDirection
            + randomOffset * randomInfluence;

        Vector2 offset = offsetDirection * amplitude;

        transform.position = _cameraPosition
                                  + new Vector3(offset.x, offset.y, 0f);
    }
    
}
