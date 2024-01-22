using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class TooltipActive : MonoBehaviour, IPointerClickHandler
    {
        public string TagName;
        public CardType CardType;
        public void OnPointerClick(PointerEventData eventData)
        {
            GetComponentInParent<Hand>().ActiveTooltip(CardType != CardType.Spells, TagName, eventData.clickCount);
        }
    }
}