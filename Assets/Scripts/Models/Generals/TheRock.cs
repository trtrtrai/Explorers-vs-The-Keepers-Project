using Controllers;
using EventArgs;
using UnityEngine;

namespace Models.Generals
{
    public sealed class TheRock : GeneralCheckSummon
    {
        private const string SummonName = "Golem";
        protected override void Start()
        {
            summonCount = 3;
            immutableCount = summonCount;
            
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
        }

        protected override void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager && ReferenceEquals(sender, WorldManager.Instance))
            {
                if (args.Team == Team && args.Character.CharacterInfo.Name.Equals(SummonName))
                {
                    var oldProgress = 1f * summonCount / immutableCount;
                    summonCount = Mathf.Clamp(summonCount - 1, 0, summonCount);
                    var newProgress = 1f * summonCount / immutableCount;
                    InvokeRequireTrigger(oldProgress, newProgress);
                    
                    if (summonCount == 0) CanSummon = true;
                }
            }
        }
        
        public override string GetDescription()
        {
            return $"Can summon after {summonCount} allies Golems were summoned on War Field.";
        }
    }
}