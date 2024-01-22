using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    public class SpellsEffect : ScriptableObject
    {
        public List<SpellsTag> SpellsTags;
    }

    public enum SpellsTag
    {
        Explorer,
        Forest,
        Rock,
        Sea,
        Volcano,
        Single,
        Boost,
        Damage,
        Multiple,
        Healing,
        Control,
        Special,
        Profit,
        Summon,
        Demolition,
        Apocalypse,
        BadEffect,
        Trap,
    }
}