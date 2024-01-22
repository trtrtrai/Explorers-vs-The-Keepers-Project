using Controllers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Energy Boost Spell", menuName = "Customs/Spells/New Energy Boost Spells")]
    public class EnergyBoostSpells : SpellsEffect
    {
        public EnergyBoostType BoostType;
        public EnergyManager Target;
        
        //BoostRegenerate
        public float AddingScale;
        public float EffectTimer;
        
        //ReduceConsume
        public int EnergyReducePerConsume;
        public int Loop;
        // more config are: card type direct, card name direct, ...
    }

    public enum EnergyBoostType
    {
        BoostRegenerate,
        ReduceConsume,
    }
}