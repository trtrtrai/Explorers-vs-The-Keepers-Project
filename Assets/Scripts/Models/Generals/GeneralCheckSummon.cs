using Controllers;
using EventArgs;
using UnityEngine;

namespace Models.Generals
{
    public abstract class GeneralCheckSummon : GeneralCheckCanSummon
    {
        [SerializeField] protected int summonCount;
        [SerializeField] protected int immutableCount;
        
        protected virtual void Start()
        {
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
        }
        
        protected virtual void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager && ReferenceEquals(sender, WorldManager.Instance))
            {
                var oldProgress = 1f * summonCount / immutableCount;
                summonCount = Mathf.Clamp(summonCount - args.Character.GetStatus().GroupNumberImmutable, 0,
                    summonCount);
                var newProgress = 1f * summonCount / immutableCount;
                InvokeRequireTrigger(oldProgress, newProgress);
                
                if (summonCount == 0) CanSummon = true;
            }
        }
    }
}