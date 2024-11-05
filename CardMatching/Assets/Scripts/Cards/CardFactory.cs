using UnityEngine;



public class CardFactory : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private CardData[] availableCards;
    [SerializeField] private Transform cardParent;
    
    private ObjectPool<Card> cardPool;
    private System.Random random;
    private void Awake()
    {
        random = new System.Random();
        InitializePool();
    }
    
    private void InitializePool()
    {
        int maxPoolSize = 36;
        cardPool = new ObjectPool<Card>(cardPrefab, maxPoolSize, cardParent);
    }
    public Card[] CreateCards(int pairCount)
    {
        CardData[] selectedPairs = SelectRandomPairs(pairCount);
        Card[] cards = new Card[pairCount * 2];

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = cardPool.Get();
            if (cards[i] != null)
            {
                cards[i].Initialize(selectedPairs[i]);
            }
        }

        ShuffleCards(cards);
        return cards;
    }

    private CardData[] SelectRandomPairs(int pairCount)
    {
        CardData[] selectedCards = new CardData[pairCount * 2];
        bool[] usedIndices = new bool[availableCards.Length];
        int selectedPairs = 0;

        while (selectedPairs < pairCount)
        {
            int randomIndex = random.Next(0, availableCards.Length);
            
            if (!usedIndices[randomIndex])
            {
                selectedCards[selectedPairs * 2] = availableCards[randomIndex];
                selectedCards[selectedPairs * 2 + 1] = availableCards[randomIndex];
                
                usedIndices[randomIndex] = true;
                selectedPairs++;
            }
        }

        return selectedCards;
    }

    private void ShuffleCards(Card[] cards)
    {
        int n = cards.Length;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Card temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
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
