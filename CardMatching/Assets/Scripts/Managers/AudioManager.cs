using System.Collections;
using System.Collections.Generic;
using Cards;
using Events;
using ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Configuration")]
        [SerializeField] private AudioLibrarySO audioLibrary;
        [SerializeField] private int audioSourcePoolSize = 5;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

        private Queue<AudioSource> audioSourcePool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            audioSourcePool = new Queue<AudioSource>();

            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                CreatePooledAudioSource();
            }

            SubscribeToEvents();
        }

        private void CreatePooledAudioSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.volume = masterVolume;
            audioSourcePool.Enqueue(source);
        }

        private AudioSource GetAudioSource()
        {
            if (audioSourcePool.Count == 0)
            {
                CreatePooledAudioSource();
            }
        
            return audioSourcePool.Dequeue();
        }

        private void ReturnAudioSource(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                audioSourcePool.Enqueue(source);
            }
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnCardSelected += PlayCardFlip;
            GameEvents.OnCardsMatched += PlayCardMatch;
            GameEvents.OnCardsMismatched += PlayCardMismatch;
            GameEvents.OnGameOver += PlayGameOver;
            GameEvents.OnGameStarted += PlayGameStart;
        }

        public void PlayCardFlip(Card card)
        {
            PlaySound(audioLibrary.cardFlip);
        }

        public void PlayCardMatch(Card card1, Card card2)
        {
            PlaySound(audioLibrary.cardMatch);
        }

        public void PlayCardMismatch(Card card1, Card card2)
        {
            PlaySound(audioLibrary.cardMismatch);
        }

        public void PlayGameOver()
        {
            PlaySound(audioLibrary.gameOver);
        }

        public void PlayGameStart(string difficultyName, GameDifficulty difficulty)
        {
            PlaySound(audioLibrary.gameStart);
        }

        private void PlaySound(AudioEventSO audioEvent)
        {
            if (audioEvent == null) return;

            AudioSource source = GetAudioSource();
            source.volume = masterVolume;
            audioEvent.PlayOneShot(source);

            StartCoroutine(ReturnToPoolAfterPlay(source));
        }

        private IEnumerator ReturnToPoolAfterPlay(AudioSource source)
        {
            while (source != null && source.isPlaying)
            {
                yield return null;
            }
        
            ReturnAudioSource(source);
        }
    
        private void OnDestroy()
        {
            GameEvents.OnCardSelected -= PlayCardFlip;
            GameEvents.OnCardsMatched -= PlayCardMatch;
            GameEvents.OnCardsMismatched -= PlayCardMismatch;
            GameEvents.OnGameOver -= PlayGameOver;
            GameEvents.OnGameStarted -= PlayGameStart;
        }
    }
}