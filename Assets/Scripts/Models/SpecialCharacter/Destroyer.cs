using System.Collections;
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
                
                StartCoroutine(AttackCoolDown(target));
            }
        }

        IEnumerator AttackCoolDown(Character target)
        {
            PlayAttack();
            float secs = characterObjs[0].GetCurrentClipLength();
            yield return new WaitForSeconds(secs);
            
            var damage = Mathf.RoundToInt(status.Hp * 0.8f); // problem if have Destroyer + Group(n): HP is TotalHealth, not individual

            target.TakeDamage(damage);
                
            TakeDamage(9999);
        }
    }
}