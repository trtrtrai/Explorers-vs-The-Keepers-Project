using System.Collections;
using UnityEngine;

namespace Models.Effects
{
    public class CameraShake : MonoBehaviour
    {
        public IEnumerator Shake(float duration, float magnitude)
        {
            var origin = transform.localPosition;
            
            var elapsed = 0f;
            while (elapsed < duration)
            {
                var x = Random.Range(-1f, 1f) * magnitude;
                var y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, origin.z);

                elapsed += Time.unscaledDeltaTime;

                yield return null;
            }

            transform.localPosition = origin;
        }
    }
}