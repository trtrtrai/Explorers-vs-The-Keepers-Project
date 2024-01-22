using System.Collections.Generic;
using UnityEngine;
using Models;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Environment Spell", menuName = "Customs/Spells/New Environment Spells")]
    public class EnvironmentSpells : SpellsEffect
    {
        public GameObject EnvironmentPrefab;
        public int DamagePerContact;
        public float EffectTimer;
        public List<CharacterTag> IgnoreList;
        public List<TileData> ListSettingUp;
    }
}