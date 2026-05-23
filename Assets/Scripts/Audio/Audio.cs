using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    
    private AudioSource _audioSource;
    [SerializeField] private bool autoDestroy;
    
    [Header("Sound properties")]
    [SerializeField, Range(-3f, 3f)] private float minPitch;
    [SerializeField, Range(-3f, 3f)] private float maxPitch;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        Assert.IsNotNull(_audioSource.clip);
        Assert.IsNotNull(_audioSource.outputAudioMixerGroup);
        
        Assert.IsTrue(minPitch <= maxPitch);
        _audioSource.pitch = Random.Range(minPitch, maxPitch);
    }

    private void Start()
    {
        if (autoDestroy)
            StartCoroutine(CoroutinePlayAndAutoDestroy());
    }

    private IEnumerator CoroutinePlayAndAutoDestroy()
    {
        _audioSource.Play();
        yield return new WaitForSeconds(_audioSource.clip.length);
        Destroy(gameObject);
    }
}
