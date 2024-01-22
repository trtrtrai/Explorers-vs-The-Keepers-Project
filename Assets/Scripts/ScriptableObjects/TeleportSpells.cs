using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Teleport Spell", menuName = "Customs/Spells/New Teleport Spells")]
    public class TeleportSpells : SpellsEffect
    {
        public TeleportType TeleportType;
        
        // Forward + Backward type
        public int Step;
    }

    public enum TeleportType
    {
        BackToHeadquarter,
        Forward,
        Backward
    }
}