using UnityEngine;

namespace Models.SpecialCharacter
{
    public class Destroyer : Character
    {
        protected override void AttackAction(TileData position, Character target)
        {
            if (target is Headquarter)
            {
                isAttacking = true;
                isMoving = false;
                
                var damage = status.Hp;

                target.TakeDamage(damage);
                
                TakeDamage(damage + Mathf.RoundToInt(status.Def * 0.75f));
            }
        }
    }
}