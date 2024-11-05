public class FlipCardCommand : IGameCommand
{
    private readonly Card card;
    private readonly bool reveal;

    public FlipCardCommand(Card card, bool reveal)
    {
        this.card = card;
        this.reveal = reveal;
    }

    public void Execute()
    {
        card.Flip(reveal);
    }

    public void Undo()
    {
        card.Flip(!reveal);
    }
}

public class MatchCardsCommand : IGameCommand
{
    private readonly Card card1;
    private readonly Card card2;

    public MatchCardsCommand(Card card1, Card card2)
    {
        this.card1 = card1;
        this.card2 = card2;
    }

    public void Execute()
    {
        card1.SetMatched();
        card2.SetMatched();
    }

    public void Undo()
    {
    }
}