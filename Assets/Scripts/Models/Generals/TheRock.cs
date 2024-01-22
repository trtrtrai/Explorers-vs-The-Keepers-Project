using Controllers;
using EventArgs;

namespace Models.Generals
{
    public sealed class TheRock : GeneralCheckSummon
    {
        private const string SummonName = "Golem";
        protected override void Start()
        {
            summonCount = 3;
            
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
        }

        protected override void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager && ReferenceEquals(sender, WorldManager.Instance))
            {
                if (args.Team == Team && args.Character.CharacterInfo.Name.Equals(SummonName))
                {
                    summonCount--;
                    if (summonCount == 0) CanSummon = true;
                }
            }
        }
    }
}