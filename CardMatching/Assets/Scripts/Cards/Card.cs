using System.Collections;
using Events;
using ScriptableObjects;
using UnityEngine;

namespace Cards
{ 
    /// <summary>
    /// Represents a single card in the memory game with flip and match mechanics
    /// </summary>
    public class Card : MonoBehaviour
    {
        [Header("Card Renderers")]
        [SerializeField] private SpriteRenderer frontRenderer;
        [SerializeField] private SpriteRenderer backRenderer;
        [SerializeField] private BoxCollider2D cardCollider;
    
        private CardData cardData;
        private bool isRevealed;
        private bool isMatched;
        private bool isInteractable;
        private Coroutine flipCoroutine;
        
        public string CardId => cardData.id;
        public bool IsRevealed => isRevealed;
        public bool IsMatched => isMatched;
        public bool IsInteractable => isInteractable;
    
        private void Awake()
        {
            if (cardCollider == null)
                cardCollider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Initializes the card with the provided card data
        /// </summary>
        public void Initialize(CardData data)
        {
            cardData = data;
            frontRenderer.sprite = data.frontSprite;
            backRenderer.sprite = data.backSprite;
            ResetCard();
        }
    
        /// <summary>
        /// Resets the card to its initial state
        /// </summary>
        public void ResetCard()
        {
            isRevealed = false;
            isMatched = false;
            isInteractable = true;
            transform.rotation = Quaternion.identity;
            frontRenderer.gameObject.SetActive(false);
            backRenderer.gameObject.SetActive(true);
            cardCollider.enabled = true;
        }
    
        private void OnMouseDown()
        {
            if (isInteractable && !isRevealed && !isMatched)
            {
                GameEvents.InvokeCardSelected(this);
            }
        }

        /// <summary>
        /// Initiates the card flip animation
        /// </summary>
        /// <param name="reveal">True to show front, false to show back</param>
        public void Flip(bool reveal)
        {
            if(flipCoroutine != null)
                StopCoroutine(flipCoroutine);
            
            flipCoroutine = StartCoroutine(FlipCoroutine(reveal));
        }

        /// <summary>
        /// Handles the card flip animation
        /// </summary>
        private IEnumerator FlipCoroutine(bool reveal)
        {
            isInteractable = false;
            float elapsed = 0f;
            float duration = 0.5f;

            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(0f, reveal ? 180f : 0f, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
            
                // Use smooth step for more natural animation
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, smoothProgress);
            
                // Switch sprites at halfway point
                if (progress >= 0.5f && isRevealed != reveal)
                {
                    frontRenderer.gameObject.SetActive(reveal);
                    backRenderer.gameObject.SetActive(!reveal);
                    isRevealed = reveal;
                }

                yield return null;
            }

            transform.rotation = endRotation;
            isInteractable = true;
            flipCoroutine = null;
        }

        /// <summary>
        /// Sets the card as matched and initiates fade out animation
        /// </summary>
        public void SetMatched()
        {
            isMatched = true;
            isInteractable = false;
            cardCollider.enabled = false;
            StartCoroutine(FadeOutCard());
        }

        /// <summary>
        /// Handles the fade out animation for matched cards
        /// </summary>
        private IEnumerator FadeOutCard()
        {
            float duration = 0.5f; 
            float elapsed = 0f;
        
            Color frontColor = frontRenderer.color;
            Color backColor = backRenderer.color;
        
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / duration;
            
                // Smooth fade out effect
                float alpha = 1f - Mathf.SmoothStep(0f, 1f, normalizedTime);
            
                // Update alpha values
                frontColor.a = alpha;
                backColor.a = alpha;
            
                frontRenderer.color = frontColor;
                backRenderer.color = backColor;
            
                yield return null;
            }
        
            // Reset colors and disable card
            gameObject.SetActive(false);
            frontColor.a = 1;
            backColor.a = 1;
            frontRenderer.color = frontColor;
            backRenderer.color = backColor;
        }
    }
}