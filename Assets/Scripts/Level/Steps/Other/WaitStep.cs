using System.Collections;
using UnityEngine;

public class WaitStep : LevelStep
{
    
    [SerializeField] private float durationTime;
    
    protected override void StartStep()
    {
        StartCoroutine(CoroutineWait());
    }

    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not WaitStep)
        {
            Debug.LogError("[GameUIStep] Notification was received by an Unity object but not a GameUIStep.");
            return;
        }
        
        OnLevelStepFinishedNotification();
    }

    private IEnumerator CoroutineWait()
    {
        yield return new WaitForSeconds(durationTime);
        OnReceivedNotificationFromUnityObject(this);
    }
}