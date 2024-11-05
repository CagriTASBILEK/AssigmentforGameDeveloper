using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Memory Game/Card Data")]
public class CardData : ScriptableObject
{
    public string id;
    public Sprite frontSprite;
    public Sprite backSprite;
}
