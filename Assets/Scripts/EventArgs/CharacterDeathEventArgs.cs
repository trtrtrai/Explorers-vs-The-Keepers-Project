namespace EventArgs
{
    public class CharacterDeathEventArgs : System.EventArgs
    {
        public readonly int RoadIndex;

        public CharacterDeathEventArgs(int roadIndex)
        {
            RoadIndex = roadIndex;
        }
    }
}