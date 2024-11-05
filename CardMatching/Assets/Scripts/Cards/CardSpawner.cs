using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardFactory cardFactory;
    private GameConfig gameConfig;
    private Card[] activeCards;

    public void Initialize(GameConfig config)
    {
        if(config == null)
        {
            Debug.LogError("GameConfig is null in CardSpawner!");
            return;
        }
        gameConfig = config;
    }

    public Card[] SpawnCards(GameDifficulty difficulty)
    {
        if(activeCards != null)
        {
            cardFactory.ReturnAllCards();
        }
        GameConfig.GridConfig gridConfig = gameConfig.GetGridConfig(difficulty);
        int totalCards = gridConfig.rows * gridConfig.columns;
        int pairCount = totalCards / 2;

        activeCards = cardFactory.CreateCards(pairCount);
        if(activeCards != null)
        {
            PositionCards(activeCards, gridConfig);
        }
        return activeCards;
    }

    private void PositionCards(Card[] cards, GameConfig.GridConfig gridConfig)
    {
        if (cards == null) return;

        float startX = -(gridConfig.columns - 1) * gridConfig.cardSpacing * 0.5f;
        float startY = (gridConfig.rows - 1) * gridConfig.cardSpacing * 0.5f;

        for (int i = 0; i < cards.Length; i++)
        {
            int row = i / gridConfig.columns;
            int col = i % gridConfig.columns;

            float xPos = startX + (col * gridConfig.cardSpacing);
            float yPos = startY - (row * gridConfig.cardSpacing);
            
            cards[i].transform.position = new Vector3(xPos, yPos, 0);
            cards[i].gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if(activeCards != null)
        {
            cardFactory.ReturnAllCards();
        }
    }
}
