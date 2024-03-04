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
            if (GetComponentInParent<Hand>() is null) return;
            GetComponentInParent<Hand>().ActiveTooltip(CardType != CardType.Spells, TagName, eventData.clickCount);
        }
    }
}