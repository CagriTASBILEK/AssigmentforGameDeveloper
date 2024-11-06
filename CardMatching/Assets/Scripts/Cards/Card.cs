using System.Collections;
using Events;
using ScriptableObjects;
using UnityEngine;

namespace Cards
{ 
    public class Card : MonoBehaviour
    {
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
        public void Initialize(CardData data)
        {
            cardData = data;
            frontRenderer.sprite = data.frontSprite;
            backRenderer.sprite = data.backSprite;
            ResetCard();
        }
    
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

        public void Flip(bool reveal)
        {
            if(flipCoroutine != null)
                StopCoroutine(flipCoroutine);
            
            flipCoroutine = StartCoroutine(FlipCoroutine(reveal));
        }

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
            
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, smoothProgress);
            
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


        public void SetMatched()
        {
            isMatched = true;
            isInteractable = false;
            cardCollider.enabled = false;
            StartCoroutine(FadeOutCard());
        }
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
            
            
                float alpha = 1f - Mathf.SmoothStep(0f, 1f, normalizedTime);
            
            
                frontColor.a = alpha;
                backColor.a = alpha;
            
                frontRenderer.color = frontColor;
                backRenderer.color = backColor;
            
                yield return null;
            }
        
            gameObject.SetActive(false);
            frontColor.a = 1;
            backColor.a = 1;
            frontRenderer.color = frontColor;
            backRenderer.color = backColor;
        }

    
    
    }
}