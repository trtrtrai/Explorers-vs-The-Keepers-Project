using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Control Spell", menuName = "Customs/Spells/New Control Spells")]
    public class ControlSpells : SpellsEffect
    {
        public ControlType ControlType;
        public float EffectTimer;
    }

    public enum ControlType
    {
        Stun,
        MindControl
    }
}