using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class GeneralRequirementTooltipActive : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            GetComponentInParent<GeneralRequirementUI>().ActiveTooltip();
        }
    }
}