using Models;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Spells used for Boost and Bad Effect tag.
    /// </summary>
    [CreateAssetMenu(fileName = "Stats Spell", menuName = "Customs/Spells/New Stats Spells")]
    public class StatsSpells : SpellsEffect
    {
        public StatsType StatsType; //except Health
        public bool IsBoost;
        public bool IsScale;
        public float Quantity;
        public float EffectTimer;
    }
}