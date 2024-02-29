using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using ScriptableObjects;
using UnityEngine;

namespace GUI.SelectCardDeck
{
    public class CardInventory : MonoBehaviour
    {
        [SerializeField] private GameObject cardSlotPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private List<CardSlot> inventorySlots;
        
        [SerializeField] private List<CardName> cardsOwned; //test

        private void Start()
        {
            inventorySlots = new List<CardSlot>();

            for (int i = 0; i < cardsOwned.Count; i++)
            {
                var slotObj = Instantiate(cardSlotPrefab, content);
                var script = slotObj.GetComponent<CardSlot>();
                inventorySlots.Add(script);
                var cardInfo = Resources.Load<CardInfo>($"ScriptableObjects/Cards/{cardsOwned[i]}Card");
                script.SetupInventoryCard(cardInfo, i);
            }
        }
    }
}