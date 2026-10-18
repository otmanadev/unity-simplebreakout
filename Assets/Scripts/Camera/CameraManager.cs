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

    [Header("Movement")] 
    private bool _isMoving;
    public event Action OnCameraMovementCompleted;

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
        Vector3 sourcePosition = _camera.transform.position;
        int sourcePixelsPerUnit = _pixelPerfectCamera.assetsPPU;

        Debug.Log($"[CameraManager] Moving camera from {sourcePosition} (PPU {sourcePixelsPerUnit}) to {targetPosition} (PPU {targetPixelsPerUnit}) in {duration} seconds.]");
        while (elapsedTime < duration)
        {
            _camera.transform.position = Vector3.Lerp(sourcePosition, 
                new Vector3(targetPosition.x, targetPosition.y, sourcePosition.z), 
                t);
            
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
    
}
