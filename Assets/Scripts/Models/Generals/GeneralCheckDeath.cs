using Controllers;
using EventArgs;
using UnityEngine;

namespace Models.Generals
{
    public abstract class GeneralCheckDeath : GeneralCheckCanSummon
    {
        [SerializeField] protected int deathCount;

        protected virtual void Start()
        {
            Debug.Log("abstract invoke");
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
        }
        
        protected virtual void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager && ReferenceEquals(sender, WorldManager.Instance))
            {
                args.Character.OnCharacterDeath += CharacterDeathDetect;
            }
        }

        protected virtual void CharacterDeathDetect(object sender, CharacterDeathEventArgs args)
        {
            if (sender is not Character character) return;

            var amount = character.GetStatus().GroupNumberImmutable;
            if (amount < 0) return;
            
            deathCount = Mathf.Clamp(deathCount - amount, 0, deathCount);
            
            if (deathCount == 0) CanSummon = true;
        }
    }
}