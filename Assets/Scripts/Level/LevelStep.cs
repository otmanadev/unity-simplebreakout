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

    protected void OnLevelStepFinishedNotification()
    {
        _stepsGroup.OnLevelStepFinishedNotification();
    }
    
}
