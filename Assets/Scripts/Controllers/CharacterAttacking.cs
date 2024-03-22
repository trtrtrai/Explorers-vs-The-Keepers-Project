using System;
using EventArgs;
using Models;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Character))]
    public class CharacterAttacking : MonoBehaviour
    {
        public event EventHandler<CharacterAttackEventArgs> OnAttack; 
        
        public void AttackEnemy(string charName, Character enemy, int damage)
        {
            if (enemy is null) return;
            
            EffectsController.Instance.TriggeredEffect(charName + "-Attack", transform.localPosition, enemy.CharacterWorldPosition());
            var realDamage = enemy.TakeDamage(damage);
            OnAttack?.Invoke(gameObject, new CharacterAttackEventArgs(enemy, realDamage));
        }
    }
}