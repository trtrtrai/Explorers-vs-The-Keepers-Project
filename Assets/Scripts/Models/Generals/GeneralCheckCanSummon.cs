using UnityEngine;

namespace Models.Generals
{
    public abstract class GeneralCheckCanSummon : MonoBehaviour
    {
        [SerializeField] protected bool CanSummon = false;
        public int Team; // check 2 card deck to initial team

        public bool GeneralCanSummon() => CanSummon;
    }
}