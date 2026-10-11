using System;
using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogManager : MonoBehaviour
{
    
    public static DialogManager Instance;

    public Action<Dialog> OnDialogFinished;

    private AudioSource _audioSource;
    
    [SerializeField] private CanvasRenderer dialogCanvasRenderer;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    
    private Dialog _currentDialog;

    private bool _audioClipDone;
    private bool _subtitlesDone;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>DialogManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(dialogCanvasRenderer);
        Assert.IsNotNull(textMeshPro);
        
        _audioSource = GetComponent<AudioSource>();

        dialogCanvasRenderer.gameObject.SetActive(false);
        textMeshPro.text = string.Empty;
    }

    /// <summary>
    /// Joue le dialogue passé en paramètre.
    /// Si un dialogue est déjà en cours, alors il ne sera pas joué.
    /// </summary>
    /// <param name="dialog"></param>
    public void PlayDialog(Dialog dialog)
    {
        if (_currentDialog != null)
            return;

        if (dialog.DialogClip == null)
            return;
        
        dialogCanvasRenderer.gameObject.SetActive(true);
        _currentDialog = dialog;
        
        StartCoroutine(CoroutineStartDialogAudio());
        StartCoroutine(CoroutineStartDialogSubtitles());
    }
    
    /// <summary>
    /// Démarre la coroutine de lecture de l'audio du dialogue.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineStartDialogAudio()
    {
        _audioSource.clip = _currentDialog.DialogClip;
        _audioSource.Play();
        
        yield return new WaitForSeconds(_currentDialog.DialogClip.length);
        
        NotifyAudioDone();
    }
    
    /// <summary>
    /// Notifie que l'audio du dialogue est terminé.
    /// </summary>
    private void NotifyAudioDone()
    {
        _audioClipDone = true;
        _audioSource.clip = null;
        
        VerifyIfDialogIsDone();
    }
    
    /// <summary>
    /// Démarre la coroutine de lecture des sous-titres du dialogue.
    /// Si aucun dialogue, alors on arrête son traitement.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineStartDialogSubtitles()
    {
        if (_currentDialog.DialogSubtitles == null)
        {
            NotifySubtitlesDone();
            yield break;
        }

        foreach (DialogSubtitle dialogSubtitle in _currentDialog.DialogSubtitles)
        {
            UpdateDialogUIText(dialogSubtitle.Text);
            yield return new WaitForSeconds(dialogSubtitle.Duration);
        }
        
        NotifySubtitlesDone();
    }

    /// <summary>
    /// Notifie que les sous-titres du dialogue sont terminés.
    /// </summary>
    private void NotifySubtitlesDone()
    {
        _subtitlesDone = true;
        UpdateDialogUIText(string.Empty);
            
        VerifyIfDialogIsDone();
    }
    
    /// <summary>
    /// Notifie les events si le dialogue est complétement terminé.
    /// </summary>
    private void VerifyIfDialogIsDone()
    {
        if (!_audioClipDone || !_subtitlesDone)
            return;

        _audioClipDone = false;
        _subtitlesDone = false;
        
        dialogCanvasRenderer.gameObject.SetActive(false);
        OnDialogFinished?.Invoke(_currentDialog);
    }

    /// <summary>
    /// Met à jour le texte dans l'UI du Dialogue.
    /// Si le texte est vide, alors le Canvas UI du dialog est désactivé.
    /// </summary>
    /// <param name="text"></param>
    public void UpdateDialogUIText(string text)
    {
        if (string.IsNullOrEmpty(text) && dialogCanvasRenderer.gameObject.activeInHierarchy)
        {
            dialogCanvasRenderer.gameObject.SetActive(false);
            textMeshPro.text = string.Empty;
            return;
        }
            
        if (!dialogCanvasRenderer.gameObject.activeSelf)
            dialogCanvasRenderer.gameObject.SetActive(true);
        textMeshPro.text = text;
    }
    
}
