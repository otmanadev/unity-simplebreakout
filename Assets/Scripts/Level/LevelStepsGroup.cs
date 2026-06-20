using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelStepsGroup
{
    
    private LevelSequence _levelSequence;
    [SerializeField] private List<LevelStep> steps;
    private int _stepsFinished;

    /// <summary>
    /// Démarre le groupe de Steps.
    /// Exécute tous les steps en parallèle.
    /// </summary>
    /// <param name="levelSequence"></param>
    public void StartSteps(LevelSequence levelSequence)
    {
        if (steps == null || steps.Count == 0)
        {
            Debug.LogError($"[LevelStepsGroup] No Steps were defined");
            return;
        }
        _stepsFinished = 0;
        _levelSequence = levelSequence;
        LaunchSteps();
    }

    private void LaunchSteps()
    {
        foreach (LevelStep step in steps)
        {
            step.StartStep(this);
        }
    }

    /// <summary>
    /// Reçoit une notification d'une step pour indiquer qu'il a été terminé.
    /// </summary>
    public void OnLevelStepFinishedNotification()
    {
        _stepsFinished += 1;
        
        Debug.Log($"[StepsGroup] Step [{_stepsFinished}/{steps.Count}] finished");
        NotifyStepsGroupIfAllStepsFinished();
    }

    private void NotifyStepsGroupIfAllStepsFinished()
    {
        if (_stepsFinished != steps.Count)
            return;
        
        _levelSequence.OnStepsGroupFinishedNotification();
    }
    
}
