using System.Collections;
using UnityEngine;

public class WaitStep : LevelStep
{
    
    [SerializeField] private float durationTime;
    
    protected override void StartStep()
    {
        StartCoroutine(CoroutineWait());
    }

    private IEnumerator CoroutineWait()
    {
        yield return new WaitForSeconds(durationTime);
        OnLevelStepFinishedNotification();
    }
}