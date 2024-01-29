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
                
                var damage = Mathf.RoundToInt(status.Hp * 0.8f);

                target.TakeDamage(damage);
                
                TakeDamage(9999);
            }
        }
    }
}