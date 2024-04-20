using Controllers;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform Parent = null;
        public GameObject Placeholder = null;
        public bool DraggableObjOutOfHand;

        [SerializeField] private Card card;
        [SerializeField] private bool removeHoldingCard;

        public bool PlaceHolderDestroy;

        private void Awake()
        {
            card = GetComponent<Card>();
            PlaceHolderDestroy = true;
            DraggableObjOutOfHand = false;
            removeHoldingCard = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Placeholder = new GameObject();//transform.parent.GetChild(transform.parent.childCount - 1).gameObject;
            PlaceHolderDestroy = false;
            Parent = transform.parent;
            if (Parent.TryGetComponent(out Hand hand)) // call in main canvas after?
            {
                hand.DisableTooltip();
            }
            Placeholder.transform.SetParent(Parent);
            Placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
            
            var layoutElement = Placeholder.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
            layoutElement.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
            layoutElement.flexibleWidth = 0;
            layoutElement.flexibleHeight = 0;
            (Placeholder.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta; 
            
            transform.SetParent(Parent.parent);
            WorldManager.Instance.CardBeginDrag(card);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            removeHoldingCard = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Placeholder is null) return;
            
            transform.position = eventData.position;

            if (!Placeholder.activeSelf) return;
            
            int siblingIndex = Parent.childCount;

            for (int i = 0; i < Parent.childCount; i++)
            {
                if (transform.position.x < Parent.transform.GetChild(i).position.x)
                {
                    siblingIndex = i;

                    if (Placeholder.transform.GetSiblingIndex() < siblingIndex)
                    {
                        siblingIndex--;
                    }
                    break;
                }
            }
            
            Placeholder.transform.SetSiblingIndex(siblingIndex);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (removeHoldingCard) return;
            
            removeHoldingCard = true;
            transform.SetParent(Parent);
            transform.SetSiblingIndex(Placeholder.transform.GetSiblingIndex());
            
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            var destroyPlaceholder = Placeholder;
            PlaceHolderDestroy = true;
            Placeholder = null;
            Destroy(destroyPlaceholder);
            
            WorldManager.Instance.CardEndDrag(card, DraggableObjOutOfHand);
        }

        private void Update()
        {
            if (!removeHoldingCard && Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
            {
                removeHoldingCard = true;
                //Debug.Log("Holding get right click");
                transform.SetParent(Parent);
                transform.SetSiblingIndex(Placeholder.transform.GetSiblingIndex());
            
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                var destroyPlaceholder = Placeholder;
                PlaceHolderDestroy = true;
                Placeholder = null;
                DraggableObjOutOfHand = false;
                Destroy(destroyPlaceholder);
                
                WorldManager.Instance.CardEndDrag(card, DraggableObjOutOfHand);
            }
        }
    }
}
