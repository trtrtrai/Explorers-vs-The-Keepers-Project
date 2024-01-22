using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Controllers.SpecialCharacterBehaviour
{
    public class ReinforcedBehaviour : CharacterBehaviour
    {
        protected override void Update()
        {
            base.Update();

            if (character.IsAttacking && character.IsMoving)
            {
                //Debug.Log("Atked");
                InvokeAttackTo(character.Position, null);
            }
        }

        public override void EnemyAroundCheck(TileData position, List<TileData> aimPath, int aim)
        {
            // nothing here
        }
    }
}