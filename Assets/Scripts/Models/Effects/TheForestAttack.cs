using System.Collections;
using UnityEngine;

namespace Models.Effects
{
    public class TheForestAttack : EffectsAction
    {
        [SerializeField] private float existTime;

        public override void Activate(Vector3 target)
        {
            transform.localPosition = target;
            
            // countdown time to destroy
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(existTime);
            
            Destroy(gameObject);
        }
    }
}