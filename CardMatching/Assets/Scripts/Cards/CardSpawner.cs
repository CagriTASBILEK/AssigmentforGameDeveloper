using ScriptableObjects;
using UnityEngine;

namespace Cards
{
    /// <summary>
    /// Handles card spawning and positioning in the game grid
    /// </summary>
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

        // Spawn cards based on grid configuration
        public Card[] SpawnCards(GameConfig.GridConfig gridConfig)
        {
            if(activeCards != null)
            {
                cardFactory.ReturnAllCards();
            }

            int totalCards = gridConfig.rows * gridConfig.columns;
            int pairCount = totalCards / 2;

            activeCards = cardFactory.CreateCards(pairCount);
            if(activeCards != null)
            {
                PositionCards(activeCards, gridConfig);
            }
            return activeCards;
        }

        // Position cards in a grid layout
        private void PositionCards(Card[] cards, GameConfig.GridConfig gridConfig)
        {
            int rows = gridConfig.rows;
            int columns = gridConfig.columns;
            float cardSpacing = gridConfig.cardSpacing; 
        
            // Calculate grid center
            float gridCenterX = (columns - 1) * cardSpacing / 2f;
            float gridCenterY = (rows - 1) * cardSpacing / 2f;
        
            // Position each card
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int index = row * columns + col;
                    if (index >= cards.Length) break; 
                
                    Vector3 cardPosition = new Vector3(
                        col * cardSpacing - gridCenterX,
                        row * cardSpacing - gridCenterY,
                        0
                    );

                    cards[index].transform.position = cardPosition;
                    cards[index].gameObject.SetActive(true);
                }
            }
            CenterCameraOnGrid(rows, columns, cardSpacing);
        }

        // Adjust camera to fit grid
        private void CenterCameraOnGrid(int rows, int columns, float cardSpacing)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera.orthographic)
            {
                float aspectRatio = (float)Screen.width / Screen.height;
                mainCamera.orthographicSize = Mathf.Max(rows * cardSpacing / 2, columns * cardSpacing / (2 * aspectRatio));
            }
            else
            {
                mainCamera.transform.position = new Vector3(0, 0, -10);
            }
        }
        
        public void ReturnAllCards()
        {
            if (cardFactory != null)
            {
                cardFactory.ReturnAllCards();
                activeCards = null;
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
}