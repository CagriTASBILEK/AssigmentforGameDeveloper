using Cards;

namespace Commands
{
    /// <summary>
    /// Command to handle card flip animations
    /// </summary>
    public class FlipCardCommand : IGameCommand
    {
        private readonly Card card;
        private readonly bool reveal;

        public FlipCardCommand(Card card, bool reveal)
        {
            this.card = card;
            this.reveal = reveal;
        }

        // Execute flip animation
        public void Execute()
        {
            card.Flip(reveal);
        }

        // Reverse flip animation
        public void Undo()
        {
            card.Flip(!reveal);
        }
    }

    /// <summary>
    /// Command to handle matched cards
    /// </summary>
    public class MatchCardsCommand : IGameCommand
    {
        private readonly Card card1;
        private readonly Card card2;

        public MatchCardsCommand(Card card1, Card card2)
        {
            this.card1 = card1;
            this.card2 = card2;
        }

        // Set both cards as matched
        public void Execute()
        {
            card1.SetMatched();
            card2.SetMatched();
        }

        // Undo not implemented for matched cards
        public void Undo()
        {
            // Matched state cannot be undone
        }
    }
}