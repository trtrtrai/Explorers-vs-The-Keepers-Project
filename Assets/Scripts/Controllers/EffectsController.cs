using System.Collections.Generic;
using System.Linq;
using Models.Effects;
using UnityEngine;

namespace Controllers
{
    public class EffectsController : MonoBehaviour
    {
        public static EffectsController Instance { get; private set; }

        [SerializeField] private List<EffectsInfo> effectsInfos;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void TriggeredEffect(string effName, Vector3 origin, Vector3 target)
        {
            var eff = effectsInfos.FirstOrDefault(e => e.effectsName.Equals(effName));

            if (eff is null) return;

            var obj = Instantiate(eff.prefab, transform);
            obj.transform.localPosition = origin + eff.offset;
            if (!effName.Contains("Card")) AudioController.Instance.Play(effName);
            obj.GetComponent<EffectsAction>().Activate(target);
        }

        public void CameraShake(float duration, float magnitude)
        {
            if (Camera.main is null) return;
            
            StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(duration, magnitude));
        }

        #if UNITY_EDITOR
        public void test(Transform target)
        {
            TriggeredEffect("BombExplosion", new Vector3(3.46f, 0f), target.localPosition);
        }
        #endif

        private void OnDisable()
        {
            Instance = null;
        }
    }
}