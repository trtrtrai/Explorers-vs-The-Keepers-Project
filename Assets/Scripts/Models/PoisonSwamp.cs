using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using EventArgs;
using ScriptableObjects;
using UnityEngine;

namespace Models
{
    public class PoisonSwamp : MonoBehaviour
    {
        public event EventHandler<EnvironmentTriggeredEventArgs> OnTriggered;
        public event EventHandler<EnvironmentDestroyEventArgs> OnDestroyed;
        
        // idea for take damage per second: save List<Character>, give character a poison counting, if character in list -> reset timer counting
        [SerializeField] private int damage;
        [SerializeField] private float timer;
        [SerializeField] private long chain;

        [SerializeField] private bool isSetup;
        
        private BitArray ignoreTag;

        public bool IsSetup => isSetup;

        public void Setup(int damagePerContact, float effectTimer, List<CharacterTag> ignore, long chainNumber)
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

            chain = chainNumber;

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

                var realDmg = character.TakeDamage(damage);
                //Debug.Log("Poison Triggered " + realDmg + " dmg.");
                OnTriggered?.Invoke(this, new EnvironmentTriggeredEventArgs(character, realDmg));
            }
        }

        private IEnumerator StartCounting()
        {
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                yield return null;
            }
            
            OnDestroyed?.Invoke(this, new EnvironmentDestroyEventArgs(chain));
            enabled = false;
            Destroy(transform.parent.gameObject, 0.25f);
        }
    }
}