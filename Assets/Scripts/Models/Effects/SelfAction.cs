using System.Collections;
using UnityEngine;

namespace Models.Effects
{
    public class SelfAction : EffectsAction
    {
        [SerializeField] private float existTime;
        
        public override void Activate(Vector3 target)
        {
            StartCoroutine(Countdown());
        }
        
        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(existTime);
            
            Destroy(gameObject);
        }
    }
}