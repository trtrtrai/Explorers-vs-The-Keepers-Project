using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class TooltipDisable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Hand hand;

        public void OnPointerClick(PointerEventData eventData)
        {
            hand.DisableTooltip();
        }
    }
}