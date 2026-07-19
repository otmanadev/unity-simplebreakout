using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonUI : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //PlaySound(exitSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //PlaySound(clickSound);
    }
    
    private void PlaySound(AudioClip clip)
    {
        Debug.Log("PLay clip " + clip.name);
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
    
}
