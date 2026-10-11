using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public class Dialog
{
    
    [SerializeField] private AudioClip dialogClip;
    public AudioClip DialogClip => dialogClip;
    
    [SerializeField] private List<DialogSubtitle> dialogSubtitles;
    public List<DialogSubtitle> DialogSubtitles => dialogSubtitles;
    
}
