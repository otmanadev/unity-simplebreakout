using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class DialogManager : Manager
{
    
    public static DialogManager Instance;
    
    [SerializeField] private CanvasRenderer dialogCanvasRenderer;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>PlatformsManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(dialogCanvasRenderer);
        Assert.IsNotNull(textMeshPro);
        
        dialogCanvasRenderer.gameObject.SetActive(false);
        textMeshPro.text = string.Empty;
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
