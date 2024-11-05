using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardFactory cardFactory;
    [SerializeField] private Transform cardContainer;
    
    private Card[] activeCards;
    private GameConfig gameConfig;

    public void Initialize(GameConfig config)
    {
        gameConfig = config;
    }

    public Card[] SpawnCards(GameDifficulty difficulty)
    {
        ClearCurrentCards();

        GameConfig.GridConfig gridConfig = gameConfig.GetGridConfig(difficulty);
        int totalCards = gridConfig.rows * gridConfig.columns;
        int pairCount = totalCards / 2;

        activeCards = cardFactory.CreateCards(pairCount);
        PositionCards(activeCards, gridConfig);

        return activeCards;
    }

    private void PositionCards(Card[] cards, GameConfig.GridConfig gridConfig)
    {
        float startX = -(gridConfig.columns - 1) * gridConfig.cardSpacing * 0.5f;
        float startY = (gridConfig.rows - 1) * gridConfig.cardSpacing * 0.5f;

        int index = 0;
        for (int row = 0; row < gridConfig.rows; row++)
        {
            for (int col = 0; col < gridConfig.columns; col++)
            {
                if (index < cards.Length)
                {
                    float xPos = startX + (col * gridConfig.cardSpacing);
                    float yPos = startY - (row * gridConfig.cardSpacing);
                    cards[index].transform.position = new Vector3(xPos, yPos, 0);
                    index++;
                }
            }
        }
    }

    private void ClearCurrentCards()
    {
        if (activeCards != null)
        {
            for (int i = 0; i < activeCards.Length; i++)
            {
                if (activeCards[i] != null)
                {
                    cardFactory.ReturnCard(activeCards[i]);
                }
            }
            activeCards = null;
        }
    }

    private void OnDestroy()
    {
        ClearCurrentCards();
    }
}
