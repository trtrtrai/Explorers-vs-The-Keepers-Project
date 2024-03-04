using Models;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI.SelectCardDeck
{
    public enum SlotType
    {
        Inventory,
        NormalSlot,
        GeneralSlot,
    }
    
    public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SlotType slotType;
        [SerializeField] private Card card;

        [SerializeField] private CardDrag cardDrag;

        public SlotType SlotType => slotType;

        private void Awake()
        {
            if (slotType == SlotType.Inventory)
            {
                card = GetComponentInChildren<Card>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotType == SlotType.Inventory || eventData.pointerDrag is null) return;
            
            if (eventData.pointerDrag.TryGetComponent(out CardDrag draggable) && draggable.Placeholder is not null && CanCardPlaced(draggable.CardType))
            {
                draggable.Parent = transform;
                card = draggable.GetComponent<Card>();

                if (cardDrag is not null)
                {
                    draggable.SameSlot = cardDrag;
                }
                cardDrag = draggable;
                draggable.Placeholder.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (slotType == SlotType.Inventory || eventData.pointerDrag is null) return;
            
            if (eventData.pointerDrag.TryGetComponent(out CardDrag draggable) && CanCardPlaced(draggable.CardType))
            {
                if (draggable.PlaceHolderDestroy) return;
                
                draggable.Parent = null;
                card = GetComponentInChildren<Card>();
                cardDrag = GetComponentInChildren<CardDrag>();
                draggable.Placeholder.SetActive(false);
            }
        }

        public bool CanCardPlaced(CardType cardType)
        {
            switch (slotType)
            {
                case SlotType.Inventory:
                    return false; // inventory self place
                case SlotType.NormalSlot when cardType is not CardType.Generals:
                    return true;
                case SlotType.GeneralSlot when cardType is CardType.Generals:
                    return true;
                default:
                    return false;
            }
        }

        public void UpdateCardDrag()
        {
            card = GetComponentInChildren<Card>();
            cardDrag = GetComponentInChildren<CardDrag>();
        }

        public void SetupInventoryCard(CardInfo cardInfo, int index) => card.SetupCard(cardInfo, index);
    }
}