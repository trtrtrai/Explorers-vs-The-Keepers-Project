using Models;

namespace EventArgs
{
    public class CharacterStatsChangeEventArgs : System.EventArgs
    {
        public readonly StatsType StatsType;
        public readonly int OldValue;
        public readonly int NewValue;
        public readonly int Immutable;
        public readonly int GroupNumber;

        public CharacterStatsChangeEventArgs(StatsType statsType, int oldValue, int newValue, int immutable, int groupNumber)
        {
            StatsType = statsType;
            OldValue = oldValue;
            NewValue = newValue;
            Immutable = immutable;
            GroupNumber = groupNumber;
        }
    }
}