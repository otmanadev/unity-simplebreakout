using System;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    
    public static PauseManager Instance;
    private readonly string VOLUME_TEXT_SEPARATOR = "  ";
    private readonly string VOLUME_TEXT_DISPLAY = "|";
    
    [Header("References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasRenderer pauseCanvasRenderer;
    [SerializeField] private CanvasRenderer optionsCanvasRenderer;
    [SerializeField] private CanvasRenderer mainMenuCanvasRenderer;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Properties")]
    [SerializeField] private PauseState pauseState;
    
    [Header("Master Volume")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    
    [Header("SFX Volume")]
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    
    [Header("UI Volume")]
    [SerializeField] private TextMeshProUGUI uiVolumeText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>PauseManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(mainCanvas);
        Assert.IsNotNull(pauseCanvasRenderer);
        Assert.IsNotNull(optionsCanvasRenderer);
        Assert.IsNotNull(mainMenuCanvasRenderer);
        Assert.IsNotNull(audioSource);
        
        Assert.IsNotNull(masterVolumeText);
        Assert.IsNotNull(sfxVolumeText);
        Assert.IsNotNull(uiVolumeText);
        
        audioSource.ignoreListenerPause = true;

        DisableMainCanvas();
    }
    
    /// <summary>
    /// Est appelé lorsque le joueur appuie sur la touche échap.
    /// </summary>
    /// <param name="callbackContext"></param>
    public void CallbackCancelAction(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started) return;

        HandlePauseState();
    }

    private void HandlePauseState()
    {
        switch(pauseState) 
        {
            case PauseState.NO_PAUSE:
                EnableMainCanvas();
                break;
            case PauseState.PAUSE:
                DisableMainCanvas();
                break;
            case PauseState.PAUSE_OPTIONS:
                EnablePauseCanvasRenderer();
                break;
            case PauseState.PAUSE_MAIN_MENU:
                EnablePauseCanvasRenderer();
                break;
            default:
                Debug.LogError("Unknown pause state: " + pauseState);
                break;
        }
    }

    public void DisableMainCanvas()
    {
        DisablePauseCanvasRenderer();
        DisableOptionsCanvasRenderer();
        DisableMainMenuCanvasRenderer();
        
        mainCanvas.gameObject.SetActive(false);
        pauseState = PauseState.NO_PAUSE;
        AudioListener.pause = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void EnableMainCanvas()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        mainCanvas.gameObject.SetActive(true);
        pauseState = PauseState.PAUSE;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        EnablePauseCanvasRenderer();
    }

    private void DisablePauseCanvasRenderer()
    {
        pauseCanvasRenderer.gameObject.SetActive(false);
    }
    
    public void EnablePauseCanvasRenderer()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        pauseState = PauseState.PAUSE;
        DisableOptionsCanvasRenderer();
        DisableMainMenuCanvasRenderer();
        pauseCanvasRenderer.gameObject.SetActive(true);
    }
    
    private void DisableOptionsCanvasRenderer()
    {
        optionsCanvasRenderer.gameObject.SetActive(false);
    }
    
    public void EnableOptionsCanvasRenderer()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        pauseState = PauseState.PAUSE_OPTIONS;
        DisablePauseCanvasRenderer();
        optionsCanvasRenderer.gameObject.SetActive(true);
    }
    
    private void DisableMainMenuCanvasRenderer()
    {
        mainMenuCanvasRenderer.gameObject.SetActive(false);
    }
    
    public void EnableMainMenuCanvasRenderer()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        pauseState = PauseState.PAUSE_MAIN_MENU;
        DisablePauseCanvasRenderer();
        mainMenuCanvasRenderer.gameObject.SetActive(true);
    }
    
    public void ResetSelectedItem()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnIncreaseAudioGroupLevel(AudioGroup audioGroup)
    {
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.Instance.IncreaseVolumeLevel(audioGroup);
    }
    
    public void OnDecreaseAudioGroupLevel(AudioGroup audioGroup)
    {
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.Instance.DecreaseVolumeLevel(audioGroup);
    }

    public void UpdateMasterVolumeLevel(int volumeLevel)
    {
        masterVolumeText.text = string.Join(VOLUME_TEXT_SEPARATOR, Enumerable.Repeat(VOLUME_TEXT_DISPLAY, volumeLevel));
    }
    
    public void UpdateSfxVolumeLevel(int volumeLevel)
    {
        sfxVolumeText.text = string.Join(VOLUME_TEXT_SEPARATOR, Enumerable.Repeat(VOLUME_TEXT_DISPLAY, volumeLevel));
    }
    
    public void UpdateUIVolumeLevel(int volumeLevel)
    {
        uiVolumeText.text = string.Join(VOLUME_TEXT_SEPARATOR, Enumerable.Repeat(VOLUME_TEXT_DISPLAY, volumeLevel));
    }
    
}
