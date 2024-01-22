using System.Collections;
using ScriptableObjects;
using UnityEngine;

namespace Models.Spells
{
    public class ControlSpellsAttach : MonoBehaviour
    {
        [SerializeField] private ControlType controlType;
        [SerializeField] private float timer;

        [SerializeField] private bool isSetup;

        private Character myself;

        public void Setup(ControlType type, float effectTimer)
        {
            if (isSetup) return;

            if (TryGetComponent(out myself))
            {
                controlType = type;
                timer = effectTimer;
                myself.DisableAttacking();

                isSetup = true;
                StartCoroutine(StartCounting());
            }
            else
            {
                Destroy(this);
            }
        }

        private IEnumerator StartCounting()
        {
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                yield return null;
            }
            
            myself.SetupAttacking();
            Destroy(this);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}