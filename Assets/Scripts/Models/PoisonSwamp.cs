using System.Collections;
using System.Collections.Generic;
using Controllers;
using ScriptableObjects;
using UnityEngine;

namespace Models
{
    public class PoisonSwamp : MonoBehaviour
    {
        // idea for take damage per second: save List<Character>, give character a poison counting, if character in list -> reset timer counting
        [SerializeField] private int damage;
        [SerializeField] private float timer;

        [SerializeField] private bool isSetup;
        
        private BitArray ignoreTag;

        public void Setup(int damagePerContact, float effectTimer, List<CharacterTag> ignore)
        {
            if (isSetup) return;

            damage = damagePerContact;
            timer = effectTimer;

            ignoreTag = null;
            var bitArr = WorldManager.Instance.CreateCharacterTag(ignore);

            foreach (bool bit in bitArr)
            {
                if (bit)
                {
                    ignoreTag = bitArr;
                    break;
                }
            }

            StartCoroutine(StartCounting());
            isSetup = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent.TryGetComponent(out Character character))
            {
                if (ignoreTag is null)
                {
                    character.TakeDamage(damage);
                    return;
                }
                
                var bitArrAnd = ignoreTag.Clone() as BitArray;
                bitArrAnd.And(character.CharacterTag);
                
                foreach (bool bit in bitArrAnd)
                {
                    if (bit) return;
                }

                character.TakeDamage(damage);
            }
        }

        private IEnumerator StartCounting()
        {
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                yield return null;
            }

            Destroy(transform.parent.gameObject);
        }
    }
}