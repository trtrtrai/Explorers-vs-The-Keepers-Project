using System.Collections;
using System.Collections.Generic;
using Controllers;
using EventArgs;
using ScriptableObjects;

namespace Models.Generals
{
    public sealed class SuperMiner : GeneralCheckDeath
    {
        protected override void Start()
        {
            deathCount = 7;
            
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
        }

        protected override void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager && ReferenceEquals(sender, WorldManager.Instance))
            {
                // args.Character.CharacterTag is null because init at Start() invoke
                var characterTag = WorldManager.Instance.CreateCharacterTag(args.Character.CharacterInfo.CharacterTags);
                
                if (characterTag[(int)CharacterTag.Defender]) args.Character.OnCharacterDeath += CharacterDeathDetect;
            }
        }
    }
}