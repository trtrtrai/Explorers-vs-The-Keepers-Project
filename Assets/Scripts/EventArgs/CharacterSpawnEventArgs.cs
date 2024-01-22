using Models;

namespace EventArgs
{
    public class CharacterSpawnEventArgs : System.EventArgs
    {
        public readonly Character Character;
        public readonly int Team;

        public CharacterSpawnEventArgs(Character character, int team)
        {
            Character = character;
            Team = team;
        }
    }
}