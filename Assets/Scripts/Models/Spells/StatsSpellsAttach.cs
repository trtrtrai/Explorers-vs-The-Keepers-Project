using System.Collections;
using UnityEngine;

namespace Models.Spells
{
    public class StatsSpellsAttach : MonoBehaviour
    {
        [SerializeField] private float _timer;
        [SerializeField] private StatsType _statsType;
        [SerializeField] private int _revertAmount;
        [SerializeField] private Character _character;

        public bool Setup(float timer, StatsType statsType, int revertAmount)
        {
            _character = GetComponent<Character>();
            if (_character is null) return false;
            
            _timer = timer;
            _statsType = statsType;
            _revertAmount = revertAmount;

            return true;
        }

        public void StartCounting()
        {
            StartCoroutine(CountingRevert());
        }

        private IEnumerator CountingRevert()
        {
            yield return new WaitForSeconds(_timer);

            _character.StatsChange(_statsType, -_revertAmount);
            Destroy(this);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}