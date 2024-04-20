using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using ScriptableObjects;
using UnityEngine;

namespace GUI.SelectCardDeck
{
    public class CardInventoryUI : MonoBehaviour
    {
        [Header("Slot")]
        [SerializeField] private GameObject cardSlotPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private List<CardSlot> inventorySlots;
        
        [Header("Card")]
        public GameObject cardTagContentPrefab;
        public GameObject cardTagPrefab;
        [SerializeField] private List<TagExplain> listTagExplain;

        [Header("Data Selected")]
        [SerializeField] private Transform cardSlots;
        [SerializeField] private List<CardName> cardsOwned; //test

        private void Start()
        {
            inventorySlots = new List<CardSlot>();

            cardsOwned = DataManager.GetCardInventory();
            for (int i = 0; i < cardsOwned.Count; i++)
            {
                var slotObj = Instantiate(cardSlotPrefab, content);
                var script = slotObj.GetComponent<CardSlot>();
                inventorySlots.Add(script);
                var cardInfo = Resources.Load<CardInfo>($"ScriptableObjects/Cards/{cardsOwned[i]}Card");
                script.SetupInventoryCard(cardInfo, i);
            }
        }
        
        private string GetEnumString(TagExplain target, bool isCharacterTag)
        {
            return isCharacterTag ? target.CharacterTag.ToString() : target.SpellsTag.ToString();
        }
        
        public TagExplain GetTagExplain(bool isCharacterTag, string tags)
        {
            return listTagExplain.FirstOrDefault(e =>
                e.IsCharacterTag == isCharacterTag && GetEnumString(e, e.IsCharacterTag).Equals(tags));
        }

        public List<CardName> GetCardSelected()
        {
            var cardDrags = cardSlots.GetComponentsInChildren<CardDrag>();

            return cardDrags.Select(d => d.CardName).ToList();
        }
        
        public CardDrag[] GetCardDragSelected() => cardSlots.GetComponentsInChildren<CardDrag>();
    }
}