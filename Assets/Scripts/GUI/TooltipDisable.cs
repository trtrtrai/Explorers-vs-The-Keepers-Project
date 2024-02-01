using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class TooltipDisable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Hand hand;
        [SerializeField] private CharacterTab _characterTab;

        public void OnPointerClick(PointerEventData eventData)
        {
            hand.DisableTooltip();
            _characterTab.DeselectCharacter();
        }
    }
}