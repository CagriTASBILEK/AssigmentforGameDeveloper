using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Cards
{
    /// <summary>
    /// Manages card creation and pooling for the memory game
    /// </summary>
    public class CardFactory : MonoBehaviour
    {
        [SerializeField] private Card cardPrefab;
        [SerializeField] private CardData[] availableCards;
        [SerializeField] private Transform cardContainer;
        private ObjectPool<Card> cardPool;

        private void Awake()
        {
            if (cardPrefab == null)
            {
                Debug.LogError("Card prefab is not assigned to CardFactory!");
                return;
            }

            if (cardContainer == null)
            {
                cardContainer = new GameObject("CardContainer").transform;
            }

            LoadCardData();
            InitializePool();
        }

        private void LoadCardData()
        {
            // Load all card data from Resources folder
            availableCards = Resources.LoadAll<CardData>("Cards");

            if (availableCards == null || availableCards.Length == 0)
            {
                Debug.LogError("No card data found in Resources/Cards folder!");
            }
        }

        private void InitializePool()
        {
            int maxPoolSize = 100;
            cardPool = new ObjectPool<Card>(cardPrefab, maxPoolSize, cardContainer);
        }

        /// <summary>
        /// Creates pairs of cards for the game
        /// </summary>
        public Card[] CreateCards(int pairCount)
        {
            if (pairCount * 2 > 100)
            {
                Debug.LogError("Requested pair count exceeds maximum pool size!");
                return null;
            }

            CardData[] selectedPairs = SelectRandomPairs(pairCount);
            if (selectedPairs == null) return null;

            Card[] cards = new Card[pairCount * 2];

            // Initialize cards with selected pairs
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = cardPool.Get();
                if (cards[i] != null)
                {
                    cards[i].Initialize(selectedPairs[i]);
                }
            }

            return cards;
        }

        private CardData[] SelectRandomPairs(int pairCount)
        {
            if (availableCards == null || availableCards.Length == 0)
            {
                Debug.LogError("No card types available!");
                return null;
            }

            CardData[] selectedCards = new CardData[pairCount * 2];
            System.Random random = new System.Random();
        
            // Fill pairs sequentially first
            int currentIndex = 0;
            for (int i = 0; i < availableCards.Length && currentIndex < pairCount; i++)
            {
                selectedCards[currentIndex * 2] = availableCards[i];
                selectedCards[currentIndex * 2 + 1] = availableCards[i];
                currentIndex++;
            }
        
            // Fill remaining pairs randomly
            for (int i = currentIndex; i < pairCount; i++)
            {
                int randomIndex = random.Next(0, availableCards.Length);
                CardData randomCard = availableCards[randomIndex];

                selectedCards[i * 2] = randomCard;
                selectedCards[i * 2 + 1] = randomCard;
            }

            // Shuffle all cards
            for (int i = selectedCards.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                CardData temp = selectedCards[i];
                selectedCards[i] = selectedCards[j];
                selectedCards[j] = temp;
            }

            return selectedCards;
        }
        
        public void ReturnCard(Card card)
        {
            if (card != null)
            {
                cardPool.Return(card);
            }
        }

        public void ReturnAllCards()
        {
            cardPool.ReturnAll();
        }
    }
}