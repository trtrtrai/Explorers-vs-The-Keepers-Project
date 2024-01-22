using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Spells used for Damage and Healing tag.
    /// </summary>
    [CreateAssetMenu(fileName = "Health Spell", menuName = "Customs/Spells/New Health Spells")]
    public class HealthEffectSpells : SpellsEffect
    {
        public bool IsDamage;
        public float Quantity;
        public bool IsScale;
        public bool IsScaleWithMaxHealth;
        public bool IsPassDefense; // Damage only
    }
}