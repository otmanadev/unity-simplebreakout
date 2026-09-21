using UnityEngine;

public class DialogStep : LevelStep
{

    [SerializeField] private Dialog dialog;
    
    protected override void StartStep()
    {
        DialogManager.Instance.SetOrUpdateLevelStep(this);
        dialog.StartDialog();
    }

    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Dialog)
        {
            Debug.LogError("[DialogStep] Notification was received by an Unity object but not a Dialog.");
            return;
        }
        
        DialogManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
}
