using System.Collections;
using Controllers;
using EventArgs;
using ScriptableObjects;
using UnityEngine;

namespace Models.Generals
{
    [RequireComponent(typeof(Character))]
    public class Reconstruct : MonoBehaviour
    {
        [SerializeField] private float healRatio;

        [SerializeField] private Character myself;

        private void Start()
        {
            myself = GetComponent<Character>();

            StartCoroutine(WaitToCharacterSetup());
        }

        private IEnumerator WaitToCharacterSetup()
        {
            while (myself.CharacterTag is null)
            {
                yield return null;
            }

            var attacking = GetComponent<CharacterAttacking>();
            attacking.OnAttack += AttackingOnAttack;
        }

        private void AttackingOnAttack(object sender, CharacterAttackEventArgs args)
        {
            if (sender is null) return;
            if (ReferenceEquals(sender, gameObject) && !args.Target.CharacterTag[(int)CharacterTag.Headquarter])
            {
                var healingQuantity = Mathf.RoundToInt(args.DamageDeal * healRatio);
                //Debug.Log("Damage deal " + args.DamageDeal + ", heal " + healingQuantity);
                myself.Healing(healingQuantity, false, false);
            }
        }
    }
}