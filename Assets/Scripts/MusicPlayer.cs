using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioClip[] audioClips;
    private AudioSource audioSource;
    private List<AudioClip> availableAudioClips;
    private AudioClip currentAudioClip;


    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioClips = Resources.LoadAll<AudioClip>("Musics");
        availableAudioClips = audioClips.ToList();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!audioSource.isPlaying)
            SelectRandomAudioClip();

        if (availableAudioClips.Count == 0)
            availableAudioClips = audioClips.ToList();
    }

    private void SelectRandomAudioClip()
    {
        currentAudioClip = availableAudioClips[Random.Range(0, availableAudioClips.Count)];
        availableAudioClips.Remove(currentAudioClip);

        audioSource.PlayOneShot(currentAudioClip);
    }
}