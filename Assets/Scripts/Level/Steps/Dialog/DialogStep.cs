using UnityEngine;

public class DialogStep : LevelStep
{

    [SerializeField] private Dialog dialog;
    
    protected override void StartStep()
    {
        DialogManager.Instance.OnDialogFinished += OnDialogFinished;
        DialogManager.Instance.PlayDialog(dialog);
    }

    public void OnDialogFinished(Dialog playedDialog)
    {
        DialogManager.Instance.OnDialogFinished -= OnDialogFinished;
        
        if (dialog != playedDialog)
            return;

        OnLevelStepFinishedNotification();
    }
    
}
