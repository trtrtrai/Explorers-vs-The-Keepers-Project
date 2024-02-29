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

        [SerializeField] private Card card;
        [SerializeField] private bool removeHoldingCard;

        public bool PlaceHolderDestroy;

        public CardType CardType => card.CardType;
        
        private void Awake()
        {
            card = GetComponent<Card>();
            removeHoldingCard = true;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Placeholder = new GameObject();//transform.parent.GetChild(transform.parent.childCount - 1).gameObject;
            PlaceHolderDestroy = false;
            Parent = transform.parent;
            Placeholder.transform.SetParent(Parent);
            Parent = PlanetManager.Instance.GetCardSelectSpace();
            
            var layoutElement = Placeholder.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
            layoutElement.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
            layoutElement.flexibleWidth = 0;
            layoutElement.flexibleHeight = 0;
            (Placeholder.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta; 
            
            transform.SetParent(Parent);

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
            
            if (Parent)
            {
                transform.SetParent(Parent);
            }
            else
            {
                transform.SetParent(Placeholder.transform.parent);
            }
            transform.localPosition = Vector3.zero;
            
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            var destroyPlaceholder = Placeholder;
            PlaceHolderDestroy = true;
            Placeholder = null;
            Destroy(destroyPlaceholder);
        }
    }
}