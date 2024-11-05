using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioEvent", menuName = "Audio/Audio Event")]
public class AudioEventSO : ScriptableObject
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] clips;

    [Header("Properties")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0.1f, 3f)] private float pitch = 1f;
    [SerializeField] private bool randomizeVolume = false;
    [SerializeField] private bool randomizePitch = false;
    [SerializeField, Range(0f, 0.5f)] private float volumeVariation = 0.1f;
    [SerializeField, Range(0f, 0.5f)] private float pitchVariation = 0.1f;

    public void PlayOneShot(AudioSource source)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
        if (clipToPlay == null) return;

        float finalVolume = randomizeVolume ? 
            volume * Random.Range(1f - volumeVariation, 1f + volumeVariation) : 
            volume;

        source.pitch = randomizePitch ? 
            pitch * Random.Range(1f - pitchVariation, 1f + pitchVariation) : 
            pitch;

        source.PlayOneShot(clipToPlay, finalVolume);
    }
}