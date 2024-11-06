using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Audio Library")]
    public class AudioLibrarySO : ScriptableObject
    {
        [Header("Game Sounds")]
        public AudioEventSO cardFlip;
        public AudioEventSO cardMatch;
        public AudioEventSO cardMismatch;
        public AudioEventSO gameOver;
        public AudioEventSO gameStart;
    }
}