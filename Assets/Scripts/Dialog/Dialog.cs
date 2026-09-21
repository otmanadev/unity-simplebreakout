using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Dialog : MonoBehaviour
{
    
    private AudioSource _audioSource;
    private bool _audioClipDone = false;
    
    [SerializeField] private List<DialogTextDuration> dialogTextDurations = new List<DialogTextDuration>();
    private int _dialogTextDurationIndex = 0;
    private bool _dialogTextDone = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        Assert.IsNotNull(_audioSource.clip);
        Assert.IsNotNull(_audioSource.outputAudioMixerGroup);
    }

    /// <summary>
    /// Démarre le dialogue, lançant la source audio et le texte à afficher à l'écran.
    /// Envoie une notification au DialogStep une fois les 2 éléments complétements terminés.
    /// </summary>
    public void StartDialog()
    {
        StartCoroutine(CoroutineStartDialogAudio());
        StartCoroutine(CoroutineStartDialogText());
    }

    private IEnumerator CoroutineStartDialogAudio()
    {
        _audioSource.Play();
        yield return new WaitForSeconds(_audioSource.clip.length);
        
        _audioClipDone = true;
        CheckIfDialogIsDone();
    }

    private IEnumerator CoroutineStartDialogText()
    {
        while (_dialogTextDurationIndex < dialogTextDurations.Count)
        {
            DialogTextDuration dialogTextDuration = dialogTextDurations[_dialogTextDurationIndex];
            DialogManager.Instance.UpdateDialogUIText(dialogTextDuration.Text);
            yield return new WaitForSeconds(dialogTextDuration.Duration);
            _dialogTextDurationIndex += 1;
        }
        
        _dialogTextDone = true;
        DialogManager.Instance.UpdateDialogUIText(string.Empty);
        CheckIfDialogIsDone();
    }

    private void CheckIfDialogIsDone()
    {
        if (!_dialogTextDone || !_audioClipDone)
            return;
        
        DialogManager.Instance.OnReceivedNotificationFromUnityObject(this);
    }
    
}
