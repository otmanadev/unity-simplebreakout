using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIStep : LevelStep
{
    
    [SerializeField] private CanvasRenderer gameUICanvasRenderer;

    [Header("Properties")] 
    [SerializeField] private bool hasToBeTransparent;
    [SerializeField] private float durationTime;
    
    protected override void StartStep()
    {
        if (gameUICanvasRenderer == null)
        {
            Debug.LogError("[GameUIStep] Cannot launch GameUIStep cause it has no reference to Game UI Canvas. Skipping this Step...");
            OnReceivedNotificationFromUnityObject(this);
            return;
        }

        StartCoroutine(CoroutineLerpGameUIAlphaColor());
    }

    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not GameUIStep)
        {
            Debug.LogError("[GameUIStep] Notification was received by an Unity object but not a GameUIStep.");
            return;
        }
        
        OnLevelStepFinishedNotification();
    }

    private IEnumerator CoroutineLerpGameUIAlphaColor()
    {
        float elapsedTime = .0f;

        if (gameUICanvasRenderer.TryGetComponent(out Image image))
        {
            float sourceAlpha = hasToBeTransparent? 1.0f : 0.0f;
            float targetAlpha = hasToBeTransparent? 0.0f : 1.0f;
            
            Color imageColor = image.color;
            
            while (elapsedTime < durationTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / durationTime;
                
                float currentAlpha = Mathf.Lerp(sourceAlpha, targetAlpha, t);
                imageColor.a = currentAlpha;
                image.color = imageColor;
            
                yield return null;
            }
        }
        else
        {
            Debug.LogError("[GameUIStep] Cannot launch GameUIStep cause it has no reference to Image from Game UI Canvas. Skipping this Step...");
        }
        
        OnReceivedNotificationFromUnityObject(this);
    }
    
}