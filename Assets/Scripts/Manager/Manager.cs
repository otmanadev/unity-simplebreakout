using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    
    [Header("Level Step")]
    private LevelStep _levelStep;
    
    /// <summary>
    /// Met à jour le Level Step pour que les notifications passent par ici.
    /// </summary>
    /// <param name="levelStep"></param>
    public void SetOrUpdateLevelStep(LevelStep levelStep)
    {
        _levelStep = levelStep;
    }

    /// <summary>
    /// Vide la référence au Level Step pour limiter les risques d'incohérences.
    /// </summary>
    public void ClearLevelStep()
    {
        _levelStep = null;
    }

    /// <summary>
    /// Recoit une notification d'un objet Unity.
    /// </summary>
    /// <param name="unityObject"></param>
    public void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (_levelStep == null)
        {
            Debug.LogError("[Manager] Ne peut pas envoyer de notifications à LevelStep puisqu'il n'est pas instancié");
            return;
        }
        _levelStep.OnReceivedNotificationFromUnityObject(unityObject);
    }
    
}