using Models;

namespace EventArgs
{
    public class CharacterSpawnEventArgs : System.EventArgs
    {
        public readonly Character Character;
        public readonly int Team;
        public readonly int RoadIndex;

        public CharacterSpawnEventArgs(Character character, int team, int roadIndex)
        {
            Character = character;
            Team = team;
            RoadIndex = roadIndex;
        }
    }
}