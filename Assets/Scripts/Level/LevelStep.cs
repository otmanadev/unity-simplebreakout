using UnityEngine;

public abstract class LevelStep : MonoBehaviour
{

    private LevelStepsGroup _stepsGroup;

    /// <summary>
    /// Démarre la step.
    /// </summary>
    /// <param name="levelStepsGroup"></param>
    public void StartStep(LevelStepsGroup levelStepsGroup)
    {
        _stepsGroup = levelStepsGroup;
        StartStep();
    }

    protected abstract void StartStep();

    /// <summary>
    /// Réception de la notification d'un objet Unity.
    /// </summary>
    /// <param name="unityObject"></param>
    public abstract void OnReceivedNotificationFromUnityObject(Object unityObject);

    protected void OnLevelStepFinishedNotification()
    {
        _stepsGroup.OnLevelStepFinishedNotification();
    }
    
}
