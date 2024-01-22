using UnityEngine;

namespace Models.SpecialCharacter
{
    public abstract class Reinforced : Character
    {
        [SerializeField] protected GameObject objectSetupPrefab;
        [SerializeField] protected int objAmount;

        protected override void AttackAction(TileData position, Character target)
        {
            
        }

        protected override void MoveAction()
        {
            
        }
    }
}