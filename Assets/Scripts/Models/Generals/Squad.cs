using System.Collections;
using UnityEngine;

namespace Models.Generals
{
    [RequireComponent(typeof(Character))]
    public class Squad : MonoBehaviour
    {
        [SerializeField] private StatsType statsType;
        [SerializeField] private int quantity;
        [SerializeField] private float range;
        [SerializeField] private float timeCall;

        [SerializeField] private Character myself;
        [SerializeField] private float timer;

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

            StartCoroutine(SkillCounting());
        }

        private IEnumerator SkillCounting()
        {
            timer = timeCall;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                yield return null;
            }
            
            ActiveSkill();
        }

        private void ActiveSkill()
        {
            Collider[] hit = new Collider[32];
            var position = transform.localPosition;
            var radius = range * 1.75f; // for hexagon width
            var size = Physics.OverlapSphereNonAlloc(position, radius, hit, 1 << gameObject.layer);

            for (int i = 0; i < size; i++)
            {
                if (hit[i].transform.parent.TryGetComponent(out Character character))
                {
                    if (ReferenceEquals(character, myself))
                    {
                        character.StatsChange(statsType, Mathf.RoundToInt(quantity / 2f));
                    }
                    else
                    {
                        character.StatsChange(statsType, quantity);
                    }
                    //Debug.Log("Commander buff " + statsType + " " + quantity + " for " + character.name);
                }
            }

            StartCoroutine(SkillCounting());
        }
    }
}