using System.Collections;
using EventArgs;
using UnityEngine;

namespace Models.Generals
{
    [RequireComponent(typeof(Character))]
    public class SDefender : MonoBehaviour
    {
        [SerializeField] private StatsType statsTypeBoost;
        [SerializeField] private float statsRatioBoostPerHit;
        [SerializeField] private float healthRatioHit;

        [SerializeField] private Character myself;
        [SerializeField] private int hit;
        [SerializeField] private int healthHit;

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

            healthHit = myself.GetStatus().Hp;
            hit = Mathf.RoundToInt(healthHit * healthRatioHit);
            healthHit -= hit;
            myself.OnCharacterStatsChange += OnCharacterStatsChange;
        }

        private void OnCharacterStatsChange(object sender, CharacterStatsChangeEventArgs args)
        {
            if (sender is Character character && ReferenceEquals(myself, character))
            {
                if (args.StatsType == StatsType.Health && healthHit >= args.NewValue)
                {
                    var lostHp = Mathf.Clamp(healthHit - args.NewValue, 0, healthHit);
                    var hitNumber = 1 + lostHp / hit;

                    for (int i = 0; i < hitNumber; i++)
                    {
                        myself.StatsChange(statsTypeBoost,
                            Mathf.RoundToInt(character.GetStatus().Def * statsRatioBoostPerHit));
                    }

                    healthHit -= hitNumber * hit;
                }
            }
        }
    }
}