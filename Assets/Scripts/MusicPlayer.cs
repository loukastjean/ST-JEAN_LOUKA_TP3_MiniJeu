using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    #region Fields

    private AudioSource audioSource;
    private AudioClip[] allClips;
    private List<AudioClip> availableClips;
    private AudioClip currentClip;

    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeAudio();
    }

    private void Update()
    {
        // Si la musique précédente a finie
        if (!audioSource.isPlaying)
            PlayNextClip();

        // Si la liste de musiques a jouer est vide, refaire la "randomization" des musiques
        if (availableClips.Count == 0)
            ResetAvailableClips();
    }

    #endregion

    #region Audio Logic

    // Recupere les musiques dans le dossier Resources/Musics dynamiquement
    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        allClips = Resources.LoadAll<AudioClip>("Musics");
        availableClips = allClips.ToList();
    }

    // Joue le prochain clip audio
    private void PlayNextClip()
    {
        currentClip = GetRandomClip();
        audioSource.PlayOneShot(currentClip);
    }

    // Retourne un clip audio random dans les availableaudios
    private AudioClip GetRandomClip()
    {
        var index = Random.Range(0, availableClips.Count);
        var clip = availableClips[index];
        availableClips.RemoveAt(index);
        return clip;
    }

    private void ResetAvailableClips()
    {
        availableClips = allClips.ToList();
    }

    #endregion
}