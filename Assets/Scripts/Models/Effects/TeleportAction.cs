using System.Collections;
using UnityEngine;

namespace Models.Effects
{
    public class TeleportAction : EffectsAction
    {
        [SerializeField] private float existTime;
        
        public override void Activate(Vector3 target)
        {
            var obj = Instantiate(gameObject, transform.parent);

            obj.transform.localPosition = target;

            StartCoroutine(Countdown());
            StartCoroutine(obj.GetComponent<TeleportAction>().Countdown());
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(existTime);
            
            Destroy(gameObject);
        }
    }
}