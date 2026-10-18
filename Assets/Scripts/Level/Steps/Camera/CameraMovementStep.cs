using UnityEngine;

public class CameraMovementStep : LevelStep
{
    
    [Header("Objectives")]
    [SerializeField] private Vector2 targetPosition;
    [SerializeField, Min(0)] private int targetPixelsPerUnit;
    [SerializeField, Min(.0f)] private float duration;
    
    protected override void StartStep()
    {
        Debug.Log("[STEP / Camera Movement] Déplacement de la caméra en cours...");

        CameraManager.Instance.OnCameraMovementCompleted += OnCameraMovementCompleted;
        CameraManager.Instance.MoveCamera(targetPosition, targetPixelsPerUnit, duration);
    }

    /// <summary>
    /// Reçoit une notification de CameraManager informant du succès du déplacement de la caméra souhaitée.
    /// </summary>
    public void OnCameraMovementCompleted()
    {
        Debug.Log("[STEP / Camera Movement] Déplacement de la caméra terminé.");
        OnLevelStepFinishedNotification();
    }
    
}
