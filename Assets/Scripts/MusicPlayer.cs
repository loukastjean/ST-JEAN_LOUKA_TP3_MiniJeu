using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    AudioClip currentAudioClip;
    AudioClip[] audioClips;
    List<AudioClip> availableAudioClips;
    AudioSource audioSource;
    
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        audioClips = Resources.LoadAll<AudioClip>("Musics");
        availableAudioClips = audioClips.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
            SelectRandomAudioClip();
        
        if (availableAudioClips.Count == 0)
            availableAudioClips = audioClips.ToList();
    }

    void SelectRandomAudioClip()
    {
        currentAudioClip = availableAudioClips[Random.Range(0, availableAudioClips.Count)];
        availableAudioClips.Remove(currentAudioClip);
        
        audioSource.PlayOneShot(currentAudioClip);
    }
}
