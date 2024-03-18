using Models;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Card Info", menuName = "Customs/New Card Info")]
    public class CardInfo : ScriptableObject
    {
        public string Name;
        public int Cost;
        public Sprite CardIcon;
        [TextArea(3, 5)] public string Description;
        public CardType CardType;
        public CardActiveType ActiveType;

        [Range(0, 3)] public int Radius;

        public SpellsEffect SpellsEffect; // maybe be a list
        public GameObject Character;
    }
    
    public enum CardActiveType
    {
        WarField,
        WarFieldEnemy,
        Road,
        Single,
        Area,
        SingleEnemy,
        AreaEnemy,
        World,
    }

    public enum CardType
    {
        Minions,
        Generals,
        Spells
    }
}