using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance;

    [SerializeField] private AudioMixer mixerGroup;
    
    [SerializeField] private int maxVolumeLevel = 10;
    [SerializeField] private int minVolumeLevel = 0;
    
    [Header("Master Volume")]
    [SerializeField] private string masterMixerGroupName = "Master";
    [SerializeField, UnityEngine.Range(0, 10)] private int masterMixerGroupVolumeLevel = 10;
    
    [Header("SFX Volume")]
    [SerializeField] private string sfxMixerGroupName = "SFX";
    [SerializeField, UnityEngine.Range(0, 10)] private int sfxMixerGroupVolumeLevel = 10;
    
    [Header("UI Volume")]
    [SerializeField] private string uiMixerGroupName = "UI";
    [SerializeField, UnityEngine.Range(0, 10)] private int uiMixerGroupVolumeLevel = 10;
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>AudioManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(mixerGroup);
    }

    private void Start()
    {
        TryUpdateMasterVolumeLevel(0);
        TryUpdateSfxVolumeLevel(0);
        TryUpdateUIVolumeLevel(0);
    }

    public void OnIncreaseMasterVolumeLevel()
    {
        IncreaseVolumeLevel(AudioGroup.MASTER);
    }
    
    public void OnIncreaseSfxVolumeLevel()
    {
        IncreaseVolumeLevel(AudioGroup.SFX);
    }
    
    public void OnIncreaseUIVolumeLevel()
    {
        IncreaseVolumeLevel(AudioGroup.UI);
    }
    
    public void OnDecreaseMasterVolumeLevel()
    {
        DecreaseVolumeLevel(AudioGroup.MASTER);
    }
    
    public void OnDecreaseSfxVolumeLevel()
    {
        DecreaseVolumeLevel(AudioGroup.SFX);
    }
    
    public void OnDecreaseUIVolumeLevel()
    {
        DecreaseVolumeLevel(AudioGroup.UI);
    }

    public void IncreaseVolumeLevel(AudioGroup audioGroup)
    {
        switch (audioGroup)
        {
            case AudioGroup.MASTER:
                TryUpdateMasterVolumeLevel(1);
                break;
            case AudioGroup.SFX:
                TryUpdateSfxVolumeLevel(1);
                break;
            case AudioGroup.UI:
                TryUpdateUIVolumeLevel(1);
                break;
            default:
                Debug.LogError("[AudioManager] Cannot increase volume. Volume group is not recognized : " + audioGroup);
                break;
        }
    }
    
    public void DecreaseVolumeLevel(AudioGroup audioGroup)
    {
        switch (audioGroup)
        {
            case AudioGroup.MASTER:
                TryUpdateMasterVolumeLevel(-1);
                break;
            case AudioGroup.SFX:
                TryUpdateSfxVolumeLevel(-1);
                break;
            case AudioGroup.UI:
                TryUpdateUIVolumeLevel(-1);
                break;
            default:
                Debug.LogError("[AudioManager] Cannot decrease volume. Volume group is not recognized : " + audioGroup);
                break;
        }
    }

    private void TryUpdateMasterVolumeLevel(int levelVolume)
    {
        if (!CanUpdateNewLevelVolume(masterMixerGroupVolumeLevel + levelVolume))
            return;
        
        masterMixerGroupVolumeLevel += levelVolume;
        UpdateVolume(masterMixerGroupName, masterMixerGroupVolumeLevel);
        
        PauseManager.Instance.UpdateMasterVolumeLevel(masterMixerGroupVolumeLevel);
    }
    
    private void TryUpdateSfxVolumeLevel(int levelVolume)
    {
        if (!CanUpdateNewLevelVolume(sfxMixerGroupVolumeLevel + levelVolume))
            return;
        
        sfxMixerGroupVolumeLevel += levelVolume;
        UpdateVolume(sfxMixerGroupName, sfxMixerGroupVolumeLevel);
        
        PauseManager.Instance.UpdateSfxVolumeLevel(sfxMixerGroupVolumeLevel);
    }
    
    private void TryUpdateUIVolumeLevel(int levelVolume)
    {
        if (!CanUpdateNewLevelVolume(uiMixerGroupVolumeLevel + levelVolume))
            return;
        
        uiMixerGroupVolumeLevel += levelVolume;
        UpdateVolume(uiMixerGroupName, uiMixerGroupVolumeLevel);
        
        PauseManager.Instance.UpdateUIVolumeLevel(uiMixerGroupVolumeLevel);
    }
    
    private bool CanUpdateNewLevelVolume(int newLevelVolume)
    {
        return newLevelVolume >= minVolumeLevel 
               && newLevelVolume <= maxVolumeLevel;
    }

    public void UpdateVolume(string group, float volumeLevel)
    {
        float db = volumeLevel == 0
            ? -80f
            : Mathf.Lerp(-20f, 0f, volumeLevel / maxVolumeLevel);
        
        mixerGroup.SetFloat(group, db);
    }
    
}
