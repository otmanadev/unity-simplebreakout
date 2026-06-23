using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public class CameraMovementStep : LevelStep
{

    private Camera _camera;
    private PixelPerfectCamera _pixelPerfectCamera;
    
    private bool _isMoving = false;
    private float _zPosition;
    
    [Header("Objectives")]
    [SerializeField] private Vector2 targetPosition;
    private Vector2 _sourcePosition;
    [SerializeField, Min(0)] private int targetPixelsPerUnit;
    private int _sourcePixelsPerUnit;

    [Header("Duration")] 
    [SerializeField, Min(.0f)] private float duration;
    private float _elapsedTime;
    
    protected override void StartStep()
    {
        Debug.Log($"[CameraMovementStep] Start Moving Camera to coordinates [{targetPosition}], zoom [{targetPixelsPerUnit}] pixels in [{duration}] seconds");
        
        _camera = Camera.main;
        if (_camera == null)
        {
            Debug.LogError($"[CameraMovementStep] Camera is not instancied, or does not exists");
            return;
        }
        
        _pixelPerfectCamera = _camera.gameObject.GetComponent<PixelPerfectCamera>();
        if (_pixelPerfectCamera == null)
        {
            Debug.LogError($"[CameraMovementStep] PixelPerfectCamera is not instancied, or does not exists");
            return;
        }

        MoveCameraToTarget();
    }

    private void MoveCameraToTarget()
    {
        _sourcePosition = _camera.transform.position;
        _zPosition = _camera.transform.position.z;
        _sourcePixelsPerUnit = _pixelPerfectCamera.assetsPPU;
        _elapsedTime = .0f;
        _isMoving = true;
    }
    
    private void Update()
    {
        if (!_isMoving)
            return;
        
        _elapsedTime += Time.deltaTime;
        
        float t = _elapsedTime / duration;
        t = Mathf.Clamp01(t);

        _camera.transform.position = Vector3.Lerp(
            new Vector3(_sourcePosition.x, _sourcePosition.y, _zPosition), 
            new Vector3(targetPosition.x, targetPosition.y, _zPosition), 
            t);
        _pixelPerfectCamera.assetsPPU = (int) Mathf.Lerp(_sourcePixelsPerUnit, targetPixelsPerUnit, t);
        
        if (t >= 1.0f)
        {
            _isMoving = false;
            _camera.transform.position = new Vector3(targetPosition.x, targetPosition.y, _zPosition);
            _pixelPerfectCamera.assetsPPU = targetPixelsPerUnit;
            OnLevelStepFinishedNotification();
            enabled = false;
        }
    }

    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        throw new System.NotImplementedException();
    }
    
}
