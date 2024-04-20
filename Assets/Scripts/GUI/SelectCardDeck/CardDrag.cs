using System;
using Controllers;
using Models;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI.SelectCardDeck
{
    public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform Parent = null;
        public GameObject Placeholder = null;
        public CardDrag SameSlot = null;

        [SerializeField] private Transform myOrigin;
        [SerializeField] private Card card;
        [SerializeField] private bool removeHoldingCard;

        public bool PlaceHolderDestroy;

        public CardType CardType => card.CardType;
        public CardName CardName => Enum.Parse<CardName>(card.Name.Replace(" ", ""));
        
        private void Awake()
        {
            card = GetComponent<Card>();
            removeHoldingCard = true;
        }

        private void Start()
        {
            myOrigin = transform.parent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Placeholder = new GameObject();//transform.parent.GetChild(transform.parent.childCount - 1).gameObject;
            PlaceHolderDestroy = false;
            Parent = transform.parent;
            Placeholder.transform.SetParent(Parent);
            
            var layoutElement = Placeholder.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
            layoutElement.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
            layoutElement.flexibleWidth = 0;
            layoutElement.flexibleHeight = 0;
            (Placeholder.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta; 
            
            transform.SetParent(PlanetManager.Instance.GetCardSelectSpace());

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            removeHoldingCard = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Placeholder is null) return;
            
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (removeHoldingCard) return;
            
            removeHoldingCard = true;

            if (Parent && SameSlot && Parent.Equals(SameSlot.transform.parent))
            {
                SameSlot.Swap(Placeholder.GetComponentInParent<CardSlot>());
                SameSlot = null;
            }
            transform.SetParent(Parent ? Parent : myOrigin);
            transform.localPosition = Vector3.zero;
            
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            var destroyPlaceholder = Placeholder;
            PlaceHolderDestroy = true;
            Placeholder = null;
            Destroy(destroyPlaceholder);
        }

        private void Swap(CardSlot parent)
        {
            //Debug.Log(name + " " + parent.SlotType);
            transform.SetParent(parent.SlotType == SlotType.Inventory ? myOrigin : parent.transform);
            parent.UpdateCardDrag();
            transform.localPosition = Vector3.zero;
        }
        
        private void Update()
        {
            if (!removeHoldingCard && Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
            {
                removeHoldingCard = true;
                //Debug.Log("Holding get right click");
                transform.SetParent(Placeholder.transform.parent); // to last parent
                transform.localPosition = Vector3.zero;
            
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                var destroyPlaceholder = Placeholder;
                PlaceHolderDestroy = true;
                Placeholder = null;
                Destroy(destroyPlaceholder);
            }
        }

        public void BackToOrigin()
        {
            transform.SetParent(myOrigin);
            transform.parent.GetComponent<CardSlot>().UpdateCardDrag();
            transform.localPosition = Vector3.zero;
        }
    }
}