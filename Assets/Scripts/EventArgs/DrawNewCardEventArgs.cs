using ScriptableObjects;

namespace EventArgs
{
    public class DrawNewCardEventArgs
    {
        public readonly CardInfo Card;
        public readonly int HandIndex;

        public DrawNewCardEventArgs(CardInfo card, int handIndex)
        {
            Card = card;
            HandIndex = handIndex;
        }
    }
}