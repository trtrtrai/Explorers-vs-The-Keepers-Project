using Controllers;
using EventArgs;

namespace Models.Generals
{
    public sealed class Commander : GeneralCheckDeath
    {
        protected override void Start()
        {
            deathCount = 15;
            
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
        }

        protected override void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager && ReferenceEquals(sender, WorldManager.Instance))
            {
                if (args.Team == Team) args.Character.OnCharacterDeath += CharacterDeathDetect;
            }
        }
    }
}