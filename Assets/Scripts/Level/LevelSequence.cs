using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelSequence
{

    [SerializeField] private List<LevelStepsGroup> stepsGroups;
    private int _index = 0;

    /// <summary>
    /// Démarre la séquence.
    /// Exécute groupe par groupe.
    /// </summary>
    public void StartSequence()
    {
        if (stepsGroups == null || stepsGroups.Count == 0)
        {
            return;
        }
        LaunchStepsGroup();
    }

    private void LaunchStepsGroup()
    {
        LevelStepsGroup stepsGroup = stepsGroups[_index];
        stepsGroup.StartSteps(this);
    }

    /// <summary>
    /// Reçoit une notification du groupe de step pour indiquer qu'il a été terminé.
    /// </summary>
    public void OnStepsGroupFinishedNotification()
    {
        Debug.Log($"[LevelSequence] Steps Group [{_index + 1}/{stepsGroups.Count}] finished");
        TryLaunchNextStepsGroupOrNotifyLevelManager();
    }

    private void TryLaunchNextStepsGroupOrNotifyLevelManager()
    {
        if (_index >= stepsGroups.Count - 1)
        {
            LevelManager.Instance.OnSequenceFinishedNotification();
            return;
        }

        _index += 1;
        LaunchStepsGroup();
    }
}
