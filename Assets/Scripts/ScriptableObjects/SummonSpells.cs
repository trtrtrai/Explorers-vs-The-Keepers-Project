using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Summon Spell", menuName = "Customs/Spells/New Summon Spells")]
    public class SummonSpells : SpellsEffect
    {
        public GameObject CharacterPrefab;
        public int Quantity;
        public int RoadIndex = -1;
        public int Team;
    }
}